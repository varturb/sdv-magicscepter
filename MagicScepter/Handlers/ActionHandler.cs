using StardewValley;
using System.Linq;
using MagicScepter.UI;
using StardewValley.Menus;
using StardewModdingAPI.Events;
using System;
using MagicScepter.Helpers;
using System.Collections.Generic;
using StardewModdingAPI;
using MagicScepter.Models;

namespace MagicScepter.Handlers
{
  public static class ActionHandler
  {
    private static bool menuInitialized = false;
    private static bool drawButton = false;
    private static List<TeleportScroll> teleportScrollsWithKeybinds = new();

    public static void HandleTeleportAction()
    {
      if (ModUtility.Config.UseOldDialogMenu)
      {
        InitializeDialogMenu();
        ShowTeleportDialog();
      }
      else
      {
        ShowTeleportMenu();
      }
    }

    private static void ShowTeleportDialog()
    {
      var teleportScrolls = ScrollHandler.GetTeleportScrolls();
      if (teleportScrolls.Count == 1)
      {
        var farmResponse = teleportScrolls.First();
        ScrollHandler.TeleportByID(farmResponse.ID);
        return;
      }

      drawButton = Context.IsMainPlayer;
      Game1.player.currentLocation.createQuestionDialogue(
        I18n.Dialog_Title(),
        teleportScrolls.FilterHiddenItems().ToResponses(),
        HandleAnswer
      );
      teleportScrollsWithKeybinds = teleportScrolls.FilterHiddenItems().FilterItemsWithoutKeybind();
    }

    private static void HandleAnswer(Farmer farmer, string responseID)
    {
      drawButton = false;
      teleportScrollsWithKeybinds = new();
      ScrollHandler.TeleportByID(responseID);
    }

    private static void ShowTeleportMenu()
    {
      var teleportScrolls = ScrollHandler.GetTeleportScrolls();
      if (teleportScrolls.Count == 1)
      {
        var farmResponse = teleportScrolls.First();
        ScrollHandler.TeleportByID(farmResponse.ID);
      }
      else
      {
        Game1.activeClickableMenu = new TeleportMenu();
      }
    }

    private static void InitializeDialogMenu()
    {
      if (Context.IsMainPlayer && !menuInitialized)
      {
        menuInitialized = true;
        ModUtility.Helper.Events.Display.MenuChanged += MenuChanged;
      }
    }

    private static void MenuChanged(object sender, MenuChangedEventArgs e)
    {
      if (e.NewMenu is DialogueBox)
      {
        ModUtility.Helper.Events.Display.RenderedActiveMenu += DrawButton;
      }
      else
      {
        ModUtility.Helper.Events.Display.RenderedActiveMenu -= DrawButton;
      }
    }

    private static void DrawButton(object sender, EventArgs e)
    {
      if (drawButton)
      {
        var button = new DialogConfigButton(teleportScrollsWithKeybinds);
        button.draw(Game1.spriteBatch);
      }
    }
  }
}