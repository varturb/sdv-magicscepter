using MagicScepter.Models;
using MagicScepter.Handlers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using StardewModdingAPI;
using System.Linq;
using MagicScepter.Helpers;
using System;

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

    public TeleportMenu(List<TeleportScroll> teleportScrolls) : base(0, 0, 400, 400, false)
    {
      this.teleportScrolls = teleportScrolls;

      ResetLayout();
      CreateComponents();

      if (Game1.options.SnappyMenus)
      {
        snapToDefaultClickableComponent();
      }
    }

    private void ResetLayout()
    {
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

      if (yPositionOnScreen + height > vpHeight)
      { // bottom off screen
        yPositionOnScreen -= yPositionOnScreen + height - vpHeight + 5;
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
          TeleportByID
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

      scrollLabelComponent = new ScrollLabelComponent(DrawHoverText)
      {
        visible = false
      };

      var threshold = Neighbourhood.CalculateThreshold(teleportScrolls.Count);
      var upID = positionHoverTextOnTop ? -1 : threshold.Down;
      var downID = positionHoverTextOnTop ? threshold.Up : -1;
      scrollLabelComponent.SetupIDs(upID, downID);

      populateClickableComponentList();
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
      if ((Game1.options.doesInputListContain(Game1.options.useToolButton, key)
        || Game1.options.doesInputListContain(Game1.options.actionButton, key))
        && TryGetSelectedScroll(out var teleportScroll))
      {
        TeleportByID(teleportScroll.ID);
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
      if (!Context.IsMainPlayer)
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
      if (!Game1.options.SnappyMenus)
      {
        return;
      }

      var gamepadStateTS = Game1.input.GetGamePadState().ThumbSticks;
      var rightThumbStickUsed = (double)Math.Abs(gamepadStateTS.Right.X) > 0.5 || (double)Math.Abs(gamepadStateTS.Right.Y) > 0.5;
      var leftThumbStickUsed = (double)Math.Abs(gamepadStateTS.Left.X) > 0.5 || (double)Math.Abs(gamepadStateTS.Left.Y) > 0.5;

      if (!(leftThumbStickUsed || rightThumbStickUsed))
      {
        return;
      }

      var thumbstickPosition = rightThumbStickUsed
           ? new Vector2(gamepadStateTS.Right.X, gamepadStateTS.Right.Y)
           : new Vector2(gamepadStateTS.Left.X, gamepadStateTS.Left.Y);
      thumbstickPosition.Y *= -1f;
      thumbstickPosition.Normalize();

      var temp = -1f;
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
          selectedID = s.ID;
        }
      });
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
    }

    public override void receiveKeyPress(Keys key)
    {
      HandleKeybind(key);
      HandleUseToolButton(key);
      HandleMoveKeys(key);
      base.receiveKeyPress(key);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      scrollComponents.ForEach(s => s.OnClick(x, y));
      scrollLabelComponent.OnClick(x, y);
    }

    public override void receiveScrollWheelAction(int direction)
    {
      HandleScrollWheel(direction);

      base.receiveScrollWheelAction(direction);
    }

    public override void update(GameTime time)
    {
      HandleThumbstics();

      scrollComponents.ForEach(s => s.Update(time, CheckIfSelected(s.ID), Bounds));

      var y = positionHoverTextOnTop ? yPositionOnScreen - 100 : yPositionOnScreen + height + 40;
      var x = xPositionOnScreen + width / 2;
      scrollLabelComponent.UpdatePosition(x, y);

      if (TryGetSelectedScrollComponent(out var scroll))
      {
        currentlySnappedComponent = scroll;
        if (Game1.options.SnappyMenus)
        {
          base.snapCursorToCurrentSnappedComponent();
        }
      }
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

      drawMouse(b);
    }
  }
}
