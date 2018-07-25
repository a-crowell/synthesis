#pragma once

#include <string>
#include "XmlWriter.h"

namespace BXDJ
{
	class Joint;

	class Driver : public XmlWritable
	{
	public:
		enum Type : char
		{
			UNKNOWN = 0,
			MOTOR = 1,
			SERVO = 2,
			WORM_SCREW = 3,
			BUMPER_PNEUMATIC = 4,
			RELAY_PNEUMATIC = 5,
			DUAL_MOTOR = 6,
			ELEVATOR = 7
		};

		Type type;

		enum Signal : char
		{
			PWM = 1,
			CAN = 2
		};

		Signal portSignal;
		int portA;
		int portB;

		float inputGear;
		float outputGear;

		Driver(const Driver &);
		Driver(Joint *, Type type = UNKNOWN);

		void write(XmlWriter &) const;


	private:
		Joint * joint;

		static std::string toString(Type type);
		static std::string toString(Signal type);

	};
}
