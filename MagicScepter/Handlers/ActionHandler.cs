using StardewValley;
using System.Linq;
using MagicScepter.UI;
using StardewValley.Menus;
using StardewModdingAPI.Events;
using System;
using MagicScepter.Constants;
using MagicScepter.Helpers;

namespace MagicScepter.Handlers
{
  public static class ActionHandler
  {
    private static bool menuInitialized = false;
    private static bool drawButton = false;

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
      var responses = ResponseHandler.GetResponses();
      if (responses.Count == 2)
      {
        var farmResponseKey = responses.First().responseKey;
        ResponseHandler.HandleResponse(farmResponseKey);
        return;
      }

      drawButton = true;
      Game1.player.currentLocation.createQuestionDialogue(
        TranslatedKeys.Title,
        responses.ToArray(),
        HandleAnswer
      );
    }

    private static void HandleAnswer(Farmer farmer, string answer)
    {
      drawButton = false;
      ResponseHandler.HandleResponse(answer);
    }

    private static void ShowTeleportMenu()
    {
      var teleportScrolls = ResponseHandler.GetTeleportScrolls();
      if (teleportScrolls.Count == 1)
      {
        var farmResponse = teleportScrolls.First();
        ResponseHandler.HandleResponse(farmResponse.ID);
      }
      else
      {
        teleportScrolls = teleportScrolls.FilterHiddenItems();
        Game1.activeClickableMenu = new TeleportMenu(teleportScrolls);
      }
    }

    private static void InitializeDialogMenu()
    {
      if (!menuInitialized)
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
        var button = new DialogConfigButton();
        button.draw(Game1.spriteBatch);
      }
    }
  }
}