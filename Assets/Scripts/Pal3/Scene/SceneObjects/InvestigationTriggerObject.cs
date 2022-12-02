// ---------------------------------------------------------------------------------------------
//  Copyright (c) 2021-2022, Jiaqi Liu. All rights reserved.
//  See LICENSE file in the project root for license information.
// ---------------------------------------------------------------------------------------------

namespace Pal3.Scene.SceneObjects
{
    using System.Collections;
    using Command;
    using Command.SceCommands;
    using Common;
    using Core.DataReader.Scn;
    using Data;
    using MetaData;
    using UnityEngine;

    [ScnSceneObject(ScnSceneObjectType.Pushable)]
    [ScnSceneObject(ScnSceneObjectType.VirtualInvestigationTrigger)]
    [ScnSceneObject(ScnSceneObjectType.InvestigationTriggerObject)]
    public class InvestigationTriggerObject : SceneObject
    {
        private const float MAX_INTERACTION_DISTANCE = 5f;

        public InvestigationTriggerObject(ScnObjectInfo objectInfo, ScnSceneInfo sceneInfo)
            : base(objectInfo, sceneInfo)
        {
        }

        public override GameObject Activate(GameResourceProvider resourceProvider, Color tintColor)
        {
            if (Activated) return GetGameObject();
            GameObject sceneGameObject = base.Activate(resourceProvider, tintColor);
            if (ObjectInfo.IsNonBlocking == 0)
            {
                sceneGameObject.AddComponent<SceneObjectMeshCollider>(); // Add collider to block player
            }
            return sceneGameObject;
        }

        public override bool IsInteractable(InteractionContext ctx)
        {
            return Activated && ctx.DistanceToActor < MAX_INTERACTION_DISTANCE;
        }

        public override IEnumerator Interact(bool triggerredByPlayer)
        {
            if (!IsInteractableBasedOnTimesCount()) yield break;

            if (triggerredByPlayer)
            {
                CommandDispatcher<ICommand>.Instance.Dispatch(
                    new ActorStopActionAndStandCommand(ActorConstants.PlayerActorVirtualID));
            }

            yield return ExecuteScriptAndWaitForFinishIfAny();
        }
    }
}