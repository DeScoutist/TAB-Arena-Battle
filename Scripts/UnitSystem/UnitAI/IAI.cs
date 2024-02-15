
using UnityEngine;

namespace UnitSystem.UnitAI
{
	public interface IAI
	{
		UnityEngine.Transform transform { get; }
		Vector3 SpellTargetPosition { get; }
		Quaternion SpellTargetRotation { get; }
	}
}