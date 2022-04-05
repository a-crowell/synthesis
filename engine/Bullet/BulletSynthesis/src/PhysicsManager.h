#ifndef SYNTHESIS_PHYSICS_MANAGER
#define SYNTHESIS_PHYSICS_MANAGER

#include "btBulletDynamicsCommon.h"
// #include "BulletCollision/CollisionDispatch/btCollisionWorld.h"
#include <iostream>
#include <fstream>
#include <string>
#include <map>
#include <vector>

namespace synthesis {

	class PhysicsManager;

	static PhysicsManager* Manager = nullptr;

	class PhysicsManager {
	private:
		PhysicsManager();
		~PhysicsManager();

		btDefaultCollisionConfiguration* collisionConfiguration;
		btCollisionDispatcher* dispatcher;
		btBroadphaseInterface* overlappingPairCache;
		btSequentialImpulseConstraintSolver* solver;
		
	public:

		btDiscreteDynamicsWorld* dynamicsWorld;

		// Eh?
		static void CreateInstance(bool forceNew = false) {
			if (Manager != nullptr) {
				if (forceNew)
					DeleteInstance();
				else
					return;
			}
			Manager = new PhysicsManager();
		}

		static void DeleteInstance() {
			delete Manager;
			Manager = nullptr;
		}

		void Step(float deltaT);
		void Run(int frames, float secondsPerFrame);
		void DestroySimulation();

		void SetNumIteration(int numIter) {
			auto solverInfo = dynamicsWorld->getSolverInfo();
			solverInfo.m_numIterations = numIter;
		}
		int GetNumIteration() {
			return dynamicsWorld->getSolverInfo().m_numIterations;
		}

		inline btVector3 getGravity() { return dynamicsWorld->getGravity(); }
		inline void setGravity(const btVector3& gravity) { dynamicsWorld->setGravity(gravity); }

		void DestroyRigidbody(btRigidBody* rb);
		void DestroyConstraint(btTypedConstraint* constraint);

		/*inline btDiscreteDynamicsWorld* getWorld() {
			return dynamicsWorld;
		}*/

		// Are these functions necessary?
		btCollisionShape* createBoxCollisionShape(btVector3 halfSize);
		btRigidBody* createStaticRigidBody(btCollisionShape* shape);
		btRigidBody* createDynamicRigidBody(btCollisionShape* shape, btScalar mass);

		btHingeConstraint* createHingeConstraint(
			btRigidBody* bodyA, btRigidBody* bodyB, btVector3 pivotA, btVector3 pivotB,
			btVector3 axisA, btVector3 axisB, float lowLim, float highLim, bool internalCollision
		);

		void deleteAssociatedConstraints(btRigidBody* body);

		// inline int getRigidBodyActivationState(btRigidBody* body) { return body->getActivationState(); }
		// inline void setRigidBodyActivationState(btRigidBody* body, int state) { body->setActivationState(state); }

		void rayCastClosest(btVector3 from, btVector3 to, btCollisionWorld::ClosestRayResultCallback& callback);

		/*int getNextId() {
			static int id = 0;
			id++;
			return id;
		}*/

		// void Run(int frames, double secondsPerFrame);
	};

}

#endif
