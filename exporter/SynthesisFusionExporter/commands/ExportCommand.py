import adsk, adsk.core, adsk.fusion, traceback
import apper
from apper import AppObjects, item_id
from ..proto.synthesis_importbuf_pb2 import *
from google.protobuf.json_format import MessageToDict, MessageToJson
from ..utils.DebugHierarchy import printHierarchy

ATTR_GROUP_NAME = "SynthesisFusionExporter"  # attribute group name for use with apper's item_id


def exportRobot():
    ao = AppObjects()

    if ao.document.dataFile is None:
        print("Error: You must save your fusion document before exporting!")
        return

    protoDocument = Document()
    fillDocument(ao, protoDocument)
    protoDocumentAsDict = MessageToDict(protoDocument)
    # printHierarchy(ao.root_comp)
    print()  # put breakpoint here


class ExportCommand(apper.Fusion360CommandBase):

    def on_execute(self, command: adsk.core.Command, inputs: adsk.core.CommandInputs, args, input_values):
        exportRobot()


# -----------Document-----------

def fillDocument(ao, protoDocument):
    fillUserMeta(ao, protoDocument.userMeta)
    fillDocumentMeta(ao, protoDocument.documentMeta)
    fillDesign(ao, protoDocument.design)


def fillUserMeta(ao, protoUserMeta):
    currentUser = ao.app.currentUser
    protoUserMeta.userName = currentUser.userName
    protoUserMeta.id = currentUser.userId
    protoUserMeta.displayName = currentUser.displayName
    protoUserMeta.email = currentUser.email


def fillDocumentMeta(ao, protoDocumentMeta):
    document = ao.document
    protoDocumentMeta.fusionVersion = document.version
    protoDocumentMeta.name = document.name
    protoDocumentMeta.versionNumber = document.dataFile.versionNumber
    protoDocumentMeta.description = document.dataFile.description
    protoDocumentMeta.id = document.dataFile.id


def fillDesign(ao, protoDesign):
    fillComponents(ao, protoDesign.components)
    fillJoints(ao, protoDesign.joints)
    fillMaterials(ao, protoDesign.materials)
    fillAppearances(ao, protoDesign.appearances)
    fillFakeRootOccurrence(ao.root_comp, protoDesign.hierarchyRoot)


def fillFakeRootOccurrence(rootComponent, protoOccur):
    protoOccur.header.uuid = item_id(rootComponent, ATTR_GROUP_NAME)
    protoOccur.header.name = rootComponent.name
    protoOccur.componentUUID = item_id(rootComponent, ATTR_GROUP_NAME)

    for childOccur in rootComponent.occurrences:
        fillOccurrence(childOccur, protoOccur.childOccurrences.add())


def fillOccurrence(occur, protoOccur):
    protoOccur.header.uuid = item_id(occur, ATTR_GROUP_NAME)
    protoOccur.header.name = occur.name
    protoOccur.isGrounded = occur.isGrounded
    fillMatrix3D(occur.transform, protoOccur.transform)

    protoOccur.componentUUID = item_id(occur.component, ATTR_GROUP_NAME)

    for childOccur in occur.childOccurrences:
        fillOccurrence(childOccur, protoOccur.childOccurrences.add())


# -----------Components-----------

def fillComponents(ao, protoComponents):
    for fusionComponent in ao.design.allComponents:
        fillComponent(fusionComponent, protoComponents.add())


def fillComponent(fusionComponent, protoComponent):
    protoComponent.header.uuid = item_id(fusionComponent, ATTR_GROUP_NAME)
    protoComponent.header.name = fusionComponent.name
    protoComponent.header.description = fusionComponent.description
    protoComponent.header.revisionId = fusionComponent.revisionId
    protoComponent.partNumber = fusionComponent.partNumber
    fillBoundingBox3D(fusionComponent.boundingBox, protoComponent.boundingBox)
    protoComponent.materialId = fusionComponent.material.id
    fillPhysicalProperties(fusionComponent.physicalProperties, protoComponent.physicalProperties)

    # ADD: fillMeshBodies ---> see method
    # for childMesh in childComponent.meshBodies:
    #     fillMeshBodies(childMesh, component.meshBodies.add())


def fillMeshBody(fusionMeshBody, protoMeshBody):
    protoMeshBody.header.uuid = item_id(fusionMeshBody, ATTR_GROUP_NAME)
    protoMeshBody.header.name = fusionMeshBody.name
    protoMeshBody.appearanceId = fusionMeshBody.appearance.id
    protoMeshBody.materialId = fusionMeshBody.material.id
    fillPhysicalProperties(fusionMeshBody.physicalProperties, protoMeshBody.physicalProperties)
    fillBoundingBox3D(fusionMeshBody.boundingBox, protoMeshBody.boundingBox)
    # ADD: triangleMesh


def fillTriangleMesh(fusionTriMesh, protoTriMesh):
    pass


def fillPhysicalProperties(fusionPhysical, protoPhysical):
    protoPhysical.density = fusionPhysical.density
    protoPhysical.mass = fusionPhysical.mass
    protoPhysical.volume = fusionPhysical.volume
    protoPhysical.area = fusionPhysical.area
    fillVector3D(fusionPhysical.centerOfMass, protoPhysical.centerOfMass)


# -----------Joints-----------

def fillJoints(ao, protoJoints):
    for fusionJoint in ao.root_comp.allJoints:
        if isJointCorrupted(fusionJoint): continue
        fillJoint(fusionJoint, protoJoints.add())


def isJointCorrupted(fusionJoint):
    if fusionJoint.occurrenceOne is None and fusionJoint.occurrenceTwo is None:
        print("WARNING: Ignoring corrupted joint!")
        return True
    return False


def fillJoint(fusionJoint, protoJoint):
    protoJoint.header.uuid = item_id(fusionJoint, ATTR_GROUP_NAME)
    protoJoint.header.name = fusionJoint.name
    fillVector3D(getJointOrigin(fusionJoint), protoJoint.origin)
    protoJoint.isLocked = fusionJoint.isLocked
    protoJoint.isSuppressed = fusionJoint.isSuppressed

    # If occurrenceOne or occurrenceTwo is null, the joint is jointed to the root component
    protoJoint.occurrenceOneUUID = getJointedOccurrenceUUID(fusionJoint, fusionJoint.occurrenceOne)
    protoJoint.occurrenceTwoUUID = getJointedOccurrenceUUID(fusionJoint, fusionJoint.occurrenceTwo)

    # todo: fillJointMotion


def getJointOrigin(fusionJoint):
    geometryOrOrigin = fusionJoint.geometryOrOriginOne if fusionJoint.geometryOrOriginOne.objectType == 'adsk::fusion::JointGeometry' else fusionJoint.geometryOrOriginTwo
    if geometryOrOrigin.objectType == 'adsk::fusion::JointGeometry':
        return geometryOrOrigin.origin
    else:  # adsk::fusion::JointOrigin
        origin = geometryOrOrigin.geometry.origin
        return adsk.core.Point3D.create(  # todo: Is this the correct way to calculate a joint origin's true location? Why isn't this exposed in the API?
            origin.x + geometryOrOrigin.offsetX.value,
            origin.y + geometryOrOrigin.offsetY.value,
            origin.z + geometryOrOrigin.offsetZ.value)


def getJointedOccurrenceUUID(fusionJoint, fusionOccur):
    if fusionOccur is None:
        return item_id(fusionJoint.parentComponent, ATTR_GROUP_NAME)  # If the occurrence of a joint is null, the joint is jointed to the parent component (which should always be the root object)
    return item_id(fusionOccur, ATTR_GROUP_NAME)


# -----------Materials-----------

def fillMaterials(ao, protoMaterials):
    for childMaterial in ao.design.materials:
        fillMaterial(childMaterial, protoMaterials.add())


def fillMaterial(childMaterial, protoMaterial):
    protoMaterial.id = childMaterial.id
    protoMaterial.name = childMaterial.name
    protoMaterial.appearanceId = childMaterial.appearance.id
    # add protobuf def: MaterialProperties properties
    # fillMaterialsProperties()


def fillMaterialsProperties(fusionMaterials, protoMaterials):
    protoMaterials.density = fusionMaterials.density
    protoMaterials.yieldStrength = fusionMaterials.yieldStrength
    protoMaterials.tensileStrength = fusionMaterials.tensileStrength


# -----------Appearances-----------

def fillAppearances(ao, protoAppearances):
    for childAppearance in ao.design.appearances:
        fillAppearance(childAppearance, protoAppearances.add())


def fillAppearance(fusionAppearance, protoAppearance):
    protoAppearance.id = fusionAppearance.id
    protoAppearance.name = fusionAppearance.name
    protoAppearance.hasTexture = fusionAppearance.hasTexture
    # add protobuf def: AppearanceProperties properties


def fillAppearanceProperties(fusionAppearanceProps, protoAppearanceProps):
    pass


# -----------Generic-----------

def fillColor(fusionColor, protoColor):
    pass


def fillBoundingBox3D(fusionBoundingBox, protoBoundingBox):
    fillVector3D(fusionBoundingBox.maxPoint, protoBoundingBox.maxPoint)
    fillVector3D(fusionBoundingBox.minPoint, protoBoundingBox.minPoint)


def fillVector3D(fusionVector3D, protoVector3D):
    protoVector3D.x = fusionVector3D.x
    protoVector3D.y = fusionVector3D.y
    protoVector3D.z = fusionVector3D.z


def fillMatrix3D(fusionTransform, protoTransform):
    assert len(protoTransform.cells) == 0  # Don't try to fill a matrix that's already full
    protoTransform.cells.extend(fusionTransform.asArray())
