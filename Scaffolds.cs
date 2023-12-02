using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;
using STRINGS;
using System.Runtime.InteropServices;
using System.CodeDom;

namespace Scaffolds
{
  public class Scaffold : Ladder
  {
    [Serialize]
    private bool willSelfDestruct = true;

    [Serialize]
    private float deconstructMoment = -1f;

    public SchedulerHandle deconstructHandle;

    protected override void OnPrefabInit()
    {
      base.OnPrefabInit();

      Subscribe(-905833192, OnCopySettingsDelegate);
      Subscribe(493375141, OnRefreshUserMenuDelegate);
    }


    private static readonly EventSystem.IntraObjectHandler<Scaffold> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<Scaffold>(delegate (Scaffold component, object data)
    {
      component.OnCopySettings(data);
    });
    private static readonly EventSystem.IntraObjectHandler<Scaffold> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Scaffold>(delegate (Scaffold component, object data)
    {
      component.OnRefreshUserMenu(data);
    });

    protected override void OnSpawn()
    {
      base.OnSpawn();

      if (willSelfDestruct)
      {
        float now = GameClock.Instance.GetTime();
        float timeRemaining = deconstructMoment == -1 ? ScaffoldConfig.TimeToLive : deconstructMoment - now;
        this.scheduleDeconstruct(timeRemaining);
      }
    }

    private void scheduleDeconstruct(float timeRemaining)
    {
      if (deconstructHandle.IsValid)
      {
        deconstructHandle.ClearScheduler();
      }
      deconstructHandle = GameScheduler.Instance.Schedule(nameof(Scaffold), timeRemaining, new Action<object>(Scaffold.Deconstruct), (object)this);

      deconstructMoment = GameClock.Instance.GetTime() + timeRemaining;
    }

    private void unscheduleDeconstruct()
    {
      if (deconstructHandle.IsValid)
      {
        deconstructHandle.ClearScheduler();
      }

      deconstructMoment = -1f;
    }

    private static void Deconstruct(object data)
    {
      Scaffold scaffold = data as Scaffold;
      if (scaffold != null)
      {
        scaffold.GetComponent<DeconstructableScaffold>().OnDeconstruct();
      }
      else
      {
        Debug.LogError("Scaffolds attempted to automatically deconstruct something that wasn't a Scaffold or was null. Please report this error.");
      }
    }

    private void EnableSelfDestruct()
    {
      if (!willSelfDestruct)
      {
        this.scheduleDeconstruct(ScaffoldConfig.TimeToLive);
        willSelfDestruct = true;
      }
    }

    private void DisableSelfDestruct()
    {
      if (willSelfDestruct)
      {
        this.unscheduleDeconstruct();
        willSelfDestruct = false;
      }
    }

    protected override void OnCleanUp()
    {
      this.deconstructHandle.ClearScheduler();
      base.OnCleanUp();
    }


    private void OnCopySettings(object data)
    {
      GameObject gameObject = (GameObject)data;
      if (gameObject != null)
      {
        Scaffold component = gameObject.GetComponent<Scaffold>();
        if (component != null)
        {
          //this is copying settings TO the local variables from clipboard component
          if (component.willSelfDestruct)
          {
            EnableSelfDestruct();
          }
          else
          {
            DisableSelfDestruct();
          }
        }
      }
    }

    private void OnRefreshUserMenu(object data)
    {
      KIconButtonMenu.ButtonInfo button = willSelfDestruct ? new KIconButtonMenu.ButtonInfo("action_switch_toggle", ScaffoldConfig.SelfDestructButtonCancelText, DisableSelfDestruct, Action.NumActions, null, null, null, ScaffoldConfig.SelfDestructButtonCancelTooltip) : new KIconButtonMenu.ButtonInfo("action_switch_toggle", ScaffoldConfig.SelfDestructButtonText, EnableSelfDestruct, Action.NumActions, null, null, null, ScaffoldConfig.SelfDestructButtonTooltip);
      Game.Instance.userMenu.AddButton(base.gameObject, button);
    }
  }

  public class DeconstructableScaffold : Workable
  {
    // Modified deconstructable to replace the default behavior, this one will deconstruct instantly when given deconstruct order
    // However, it won't drop any resources from the building itself. This is important because it's made of vacuum and would give an error

    private static readonly EventSystem.IntraObjectHandler<DeconstructableScaffold> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<DeconstructableScaffold>(delegate (DeconstructableScaffold component, object data)
    {
      component.OnRefreshUserMenu(data);
    });
    private static readonly EventSystem.IntraObjectHandler<DeconstructableScaffold> OnDeconstructDelegate = new EventSystem.IntraObjectHandler<DeconstructableScaffold>(delegate (DeconstructableScaffold component, object data)
    {
      component.OnDeconstruct();
    });
    private CellOffset[] PlacementOffsets
    {
      get
      {
        Building component = GetComponent<Building>();
        if (component != null)
        {
          return component.Def.PlacementOffsets;
        }

        Debug.LogError("There's an error with the Scaffolds mod deconstructing. Please report this error.");
        return null;
      }
    }

    protected override void OnPrefabInit()
    {
      base.OnPrefabInit();
      Subscribe(493375141, OnRefreshUserMenuDelegate);
      Subscribe(-111137758, OnRefreshUserMenuDelegate);
      Subscribe(-790448070, OnDeconstructDelegate);

      CellOffset[][] table = OffsetGroups.InvertedStandardTable;
      CellOffset[] filter = null;
      CellOffset[][] offsetTable = OffsetGroups.BuildReachabilityTable(PlacementOffsets, table, filter);
      SetOffsetTable(offsetTable);
      // Not really sure exactly what the CellOffset stuff is about, too afraid to delete
      // from original deconstructable class
    }

    protected override void OnSpawn()
    {
      base.OnSpawn();
    }

    public void OnDeconstruct()
    {
      Scaffold scaffold = base.GetComponent<Scaffold>();

      if (scaffold.deconstructHandle.IsValid)
      {
        scaffold.deconstructHandle.ClearScheduler();
      }

      base.gameObject.DeleteObject(); // Goodbye
    }

    private void OnRefreshUserMenu(object data)
    {
      // Add deconstruct button
      KIconButtonMenu.ButtonInfo button = new KIconButtonMenu.ButtonInfo("action_deconstruct", ScaffoldConfig.DeconstructButtonText, OnDeconstruct, Action.NumActions, null, null, null, ScaffoldConfig.DeconstructButtonTooltip);
      Game.Instance.userMenu.AddButton(base.gameObject, button, 0f);
    }
  }
}