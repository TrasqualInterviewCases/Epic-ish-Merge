using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IAnimatedMoveable
{
    public UniTask MoveWithAnimation(Vector3 movementVector);
}
