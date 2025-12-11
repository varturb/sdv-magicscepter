using System;
using System.Collections.Generic;
using System.Linq;
using MagicScepter.Handlers;
using MagicScepter.Helpers;
using MagicScepter.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace MagicScepter.UI
{
  public class TeleportMenu : IClickableMenu
  {
    private readonly List<TeleportScroll> teleportScrolls;
    private List<ScrollComponent> scrollComponents;
    private ScrollLabelComponent scrollLabelComponent;
    private string selectedID = string.Empty;
    private bool positionHoverTextOnTop = false;
    private Rectangle Bounds => new(xPositionOnScreen, yPositionOnScreen, width, height);
    private Point lastMousePos;
    private const int scrollLabelOffset = 40;
    private readonly bool previewMode = false;
    private static int Width => (400 * ModUtility.Config.Radius / (float)(new ModConfig().Radius)).ToInt();
    private static int Height => (400 * ModUtility.Config.Radius / (float)(new ModConfig().Radius)).ToInt();

    public TeleportMenu(bool previewMode = false) : base(0, 0, 400, 400, false)
    {
      width = Width;
      height = Height;

      this.previewMode = previewMode;
      teleportScrolls = ScrollHandler.GetTeleportScrolls().FilterHiddenItems(previewMode).AdjustOrder();

      ResetLayout();
      CreateComponents();

      if (Game1.options.SnappyMenus)
      {
        snapToDefaultClickableComponent();
      }

      if (previewMode)
      {
        selectedID = scrollComponents.First().ID;
      }
    }

    private void ResetLayout()
    {
      width = Width;
      height = Height;
      var menuPositiononScreen = GetMenuPositionOnScreen();
      xPositionOnScreen = (int)menuPositiononScreen.X;
      yPositionOnScreen = (int)menuPositiononScreen.Y;

      var vpWidth = (int)Utility.ModifyCoordinateForUIScale(Game1.viewport.Width);
      var vpHeight = (int)Utility.ModifyCoordinateForUIScale(Game1.viewport.Height);

      if (xPositionOnScreen + width > vpWidth)
      { // right off screen 
        xPositionOnScreen -= xPositionOnScreen + width - vpWidth + 5;
      }

      if (xPositionOnScreen < 0)
      { // left off screen
        xPositionOnScreen -= xPositionOnScreen;
      }

      if (yPositionOnScreen + height - scrollLabelOffset > vpHeight)
      { // bottom off screen
        yPositionOnScreen -= yPositionOnScreen + height - scrollLabelOffset - vpHeight + 5;
      }

      if (yPositionOnScreen < 0)
      { // top off screen
        yPositionOnScreen -= yPositionOnScreen;
      }

      positionHoverTextOnTop = yPositionOnScreen >= vpHeight - height - 5 - 96;
    }

    private Vector2 GetMenuPositionOnScreen()
    {
      var playerStandingPosition = Game1.player.getStandingPosition();
      var offset = new Vector2(-width / 2f, -height / 2f);
      var playerYOffset = Utility.ModifyCoordinateForUIScale(-48);
      var playerCenterPoint = new Vector2(playerStandingPosition.X - (float)Game1.viewport.X, playerStandingPosition.Y - (float)Game1.viewport.Y);
      var playerCenterPositionOnScreen = Utility.ModifyCoordinatesForUIScale(playerCenterPoint);
      playerCenterPositionOnScreen.Y += playerYOffset;

      return playerCenterPositionOnScreen + offset;
    }

    private void CreateComponents()
    {
      scrollComponents = teleportScrolls.Select((tp, i) =>
      {
        var scroll = new ScrollComponent(
          tp,
          teleportScrolls.Count,
          xPositionOnScreen + width / 2,
          yPositionOnScreen + height / 2,
          tp.SpirteSource,
          TeleportByID,
          previewMode
        );

        var n = Neighbourhood.GetNeighbourhood(tp.Order, teleportScrolls.Count, ScrollLabelComponent.ComponentID, positionHoverTextOnTop);
        scroll.myID = tp.Order;
        scroll.upNeighborID = n.Up;
        scroll.leftNeighborID = n.Left;
        scroll.downNeighborID = n.Down;
        scroll.rightNeighborID = n.Right;
        scroll.fullyImmutable = true;
        return scroll;
      }).ToList();

      scrollLabelComponent = new ScrollLabelComponent(DrawHoverText, previewMode)
      {
        visible = previewMode
      };

      var threshold = Neighbourhood.CalculateThreshold(teleportScrolls.Count);
      var upID = positionHoverTextOnTop ? -1 : threshold.Down;
      var downID = positionHoverTextOnTop ? threshold.Up : -1;
      scrollLabelComponent.SetupIDs(upID, downID);

      populateClickableComponentList();

      if (!previewMode && ModUtility.Platform == GamePlatform.Android)
      {
        upperRightCloseButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - 36, yPositionOnScreen - 8, 48, 48), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f);
      }
    }

    private void DrawHoverText(SpriteBatch b, string text)
    {
      drawHoverText(b, text, Game1.smallFont);
    }

    private bool CheckIfSelected(string id)
    {
      return id == selectedID;
    }

    private bool TryGetSelectedID(out string id)
    {
      id = selectedID;
      if (selectedID.IsNotEmpty())
      {
        return true;
      }
      selectedID = string.Empty;
      return false;
    }

    private bool TryGetSelectedScrollComponent(out ScrollComponent scroll)
    {
      if (TryGetSelectedID(out var id))
      {
        scroll = scrollComponents.FirstOrDefault(s => s.ID == id);
        return true;
      }
      scroll = null;
      return false;
    }

    private ScrollComponent GetScrollComponentByID(int id)
    {
      return scrollComponents.FirstOrDefault(x => x.myID == id);
    }

    private bool TryGetSelectedScroll(out TeleportScroll teleportScroll)
    {
      if (TryGetSelectedID(out var id))
      {
        teleportScroll = teleportScrolls.FirstOrDefault(s => s.ID == id);
        return true;
      }
      teleportScroll = null;
      return false;
    }

    private void TeleportByID(string id)
    {
      if (teleportScrolls.Any(tp => tp.ID == id))
      {
        exitThisMenu(false);
        ScrollHandler.TeleportByID(id);
      }
    }

    private void HandleUseToolButton(Keys key)
    {
      if (key != Keys.None
        && (Game1.options.doesInputListContain(Game1.options.useToolButton, key) || Game1.options.doesInputListContain(Game1.options.actionButton, key))
        && TryGetSelectedScroll(out var teleportScroll))
      {
        TeleportByID(teleportScroll.ID);
      }
    }

    private static void HandleOpenConfigMenu(Keys key)
    {
      if (Game1.options.SnappyMenus && Game1.options.doesInputListContain(Game1.options.journalButton, key))
      {
        TeleportMenuConfigButton.OpenConfigMenu();
      }
    }

    private void HandleMoveKeys(Keys key)
    {
      if (Game1.options.SnappyMenus)
      {
        return;
      }

      var up = Game1.options.doesInputListContain(Game1.options.moveUpButton, key);
      var down = Game1.options.doesInputListContain(Game1.options.moveDownButton, key);
      var left = Game1.options.doesInputListContain(Game1.options.moveLeftButton, key);
      var right = Game1.options.doesInputListContain(Game1.options.moveRightButton, key);
      if (up || down || left || right)
      {
        TryGetSelectedScrollComponent(out var scroll);
        scroll ??= scrollComponents.First();
        var id = up ? scroll.upNeighborID :
                 down ? scroll.downNeighborID :
                 left ? scroll.leftNeighborID :
                 right ? scroll.rightNeighborID : -1;
        var targetScroll = id > -1 && id < ScrollLabelComponent.ComponentID ? GetScrollComponentByID(id) : scroll;
        currentlySnappedComponent = targetScroll;

        selectedID = targetScroll.ID;
        lastMousePos = Game1.getMousePosition();
      }
    }

    private void HandleKeybind(Keys key)
    {
      if (!Context.IsMainPlayer || key == Keys.None)
      {
        return;
      }

      var sButton = key.ToSButton();
      var tpScroll = teleportScrolls.FindScrollWithKeybind(sButton);
      if (tpScroll != null && tpScroll.CanTeleport && !tpScroll.Hidden)
      {
        exitThisMenu(false);
        tpScroll.Teleport();
      }
    }

    private void HandleScrollWheel(int direction)
    {
      lastMousePos = Game1.getMousePosition();

      if (selectedID.IsEmpty())
      {
        selectedID = teleportScrolls.First().ID;
        return;
      }

      TryGetSelectedScroll(out var scroll);
      if (direction > 0)
      {
        selectedID = teleportScrolls.GetNextScrollID(scroll.Order);
      }
      else if (direction < 1)
      {
        selectedID = teleportScrolls.GetPreviousScrollID(scroll.Order);
      }
    }

    private void HandleThumbstics()
    {
      if (!Game1.options.SnappyMenus || previewMode)
      {
        return;
      }

      if (!ThumbstickUsed().any)
      {
        return;
      }

      var gamepadStateTS = Game1.input.GetGamePadState().ThumbSticks;
      var thumbstickPosition = ThumbstickUsed().right
        ? new Vector2(gamepadStateTS.Right.X, gamepadStateTS.Right.Y)
        : new Vector2(gamepadStateTS.Left.X, gamepadStateTS.Left.Y);
      thumbstickPosition.Y *= -1f;
      thumbstickPosition.Normalize();

      var temp = -1f;
      var tempID = string.Empty;
      scrollComponents.ForEach(s =>
      {
        var scrollCenterPosition = new Vector2(
          s.bounds.Center.X - (xPositionOnScreen + width / 2f),
          s.bounds.Center.Y - (yPositionOnScreen + height / 2f)
        );
        var dot = Vector2.Dot(thumbstickPosition, scrollCenterPosition);
        if ((double)dot > (double)temp)
        {
          temp = dot;
          tempID = s.ID;
        }
      });

      if (tempID.IsNotEmpty())
      {
        selectedID = tempID;

        if (TryGetSelectedScrollComponent(out var scroll))
        {
          currentlySnappedComponent = scroll;
          base.snapCursorToCurrentSnappedComponent();
        }
      }
    }

    private static (bool any, bool left, bool right) ThumbstickUsed()
    {
      var gamepadStateTS = Game1.input.GetGamePadState().ThumbSticks;
      var rightThumbStickUsed = (double)Math.Abs(gamepadStateTS.Right.X) > 0.5 || (double)Math.Abs(gamepadStateTS.Right.Y) > 0.5;
      var leftThumbStickUsed = (double)Math.Abs(gamepadStateTS.Left.X) > 0.5 || (double)Math.Abs(gamepadStateTS.Left.Y) > 0.5;

      return (leftThumbStickUsed || rightThumbStickUsed, leftThumbStickUsed, rightThumbStickUsed);
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
      ResetLayout();
    }

    public override void populateClickableComponentList()
    {
      allClickableComponents ??= new();
      allClickableComponents.Clear();
      allClickableComponents.AddRange(scrollComponents);
      allClickableComponents.AddRange(scrollLabelComponent.ClickableComponents);
    }

    public override void snapToDefaultClickableComponent()
    {
      currentlySnappedComponent = scrollComponents.First();
      base.snapCursorToCurrentSnappedComponent();
    }

    public override void performHoverAction(int x, int y)
    {
      if (lastMousePos == new Point(x, y))
      {
        return;
      }

      selectedID = string.Empty;
      scrollLabelComponent.PerformHoverAction(x, y);
      scrollComponents.ForEach(s =>
      {
        s.tryHover(x, y);
        if (s.Hovered)
        {
          selectedID = s.ID;
        }
      });

      if (!scrollLabelComponent.visible && selectedID.IsNotEmpty())
      {
        scrollLabelComponent.visible = true;
      }

      base.performHoverAction(x, y);
    }

    public override void receiveKeyPress(Keys key)
    {
      if (previewMode)
      {
        return;
      }

      HandleOpenConfigMenu(key);
      HandleKeybind(key);
      HandleUseToolButton(key);
      HandleMoveKeys(key);

      if (!ThumbstickUsed().any)
      {
        base.receiveKeyPress(key);
      }
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (previewMode)
      {
        return;
      }

      scrollComponents.ForEach(s => s.OnClick(x, y));
      scrollLabelComponent.OnClick(x, y);

      base.receiveLeftClick(x, y, playSound);
    }

    public override void receiveScrollWheelAction(int direction)
    {
      if (previewMode || Game1.options.SnappyMenus)
      {
        return;
      }

      HandleScrollWheel(direction);

      base.receiveScrollWheelAction(direction);
    }

    public override void update(GameTime time)
    {
      HandleThumbstics();

      scrollComponents.ForEach(s => s.Update(time, CheckIfSelected(s.ID), Bounds));

      ResetLayout();
      var y = positionHoverTextOnTop ? yPositionOnScreen - 60 - scrollLabelOffset : yPositionOnScreen + height + scrollLabelOffset;
      var x = xPositionOnScreen + width / 2;
      scrollLabelComponent.UpdatePosition(x, y);
    }

    public override void draw(SpriteBatch b)
    {
      scrollComponents.ForEach(s =>
      {
        if (!CheckIfSelected(s.ID))
        {
          s.draw(b);
        }
      });

      if (TryGetSelectedScrollComponent(out var selectedScroll) && TryGetSelectedScroll(out var teleportScroll))
      {
        selectedScroll.draw(b);
        scrollLabelComponent.SetLabel(teleportScroll.Text);
      }
      else
      {
        scrollLabelComponent.SetDefaultLabel();
      }
      scrollLabelComponent.Draw(b);

      base.draw(b);

      drawMouse(b);
    }
  }
}
