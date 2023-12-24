using UnityEngine;

namespace Gameplay.MergeableSystem
{
	[CreateAssetMenu(menuName = "MergeableData")]
	public class MergeableDataSO : ScriptableObject
	{
		public MergeableType MergeType;
		public int Level;
	} 
}
