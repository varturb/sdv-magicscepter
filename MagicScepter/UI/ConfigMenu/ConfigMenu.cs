using System;
using System.Collections.Generic;
using System.Linq;
using MagicScepter.Constants;
using MagicScepter.Handlers;
using MagicScepter.Helpers;
using MagicScepter.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace MagicScepter.UI
{
  public class ConfigMenu : IClickableMenu
  {
    public List<WarpObject> warpObjects;
    private readonly Texture2D spritesheetTexture;
    private readonly List<VisibilityButton> visibilityButtons = new();
    private readonly List<RenameButton> renameButtons = new();
    private readonly List<MoveUpButton> moveUpButtons = new();
    private readonly List<MoveDownButton> moveDownButtons = new();
    private readonly List<ClickableTextureComponent> keybindButtons = new();
    private readonly List<ClickableComponent> rows = new();
    private ClickableTextureComponent scrollUpButton;
    private ClickableTextureComponent scrollDownButton;
    private ClickableTextureComponent scrollBar;
    private Rectangle scrollBarRunner;
    private int topRowIndex = 0;
    private const int moveUpButtonOffset = 100;
    private const int moveDownButtonOffset = 200;
    private const int hideButtonOffset = 300;
    private const int renameButtonOffset = 400;
    private const int keybindButtonOffset = 500;
    private const int pageSize = 6;
    private string hoverText = string.Empty;
    private bool scrolling;

    public ConfigMenu()
    {
      width = 832;
      height = 576;

      var topLeft = Utility.getTopLeftPositionForCenteringOnScreen(width, height);
      xPositionOnScreen = (int)topLeft.X;
      yPositionOnScreen = (int)topLeft.Y + 32;

      warpObjects = ResponseHandler.GetWarpObjects();

      spritesheetTexture = ModUtility.Helper.ModContent.Load<Texture2D>(PathConstants.SpritesheetTexturePath);

      Init();
    }

    public void RefreshWarpObjects()
    {
      warpObjects = ResponseHandler.GetWarpObjects();
      CreateComponents();
      SetScrollBarToCurrentIndex();
      populateClickableComponentList();
    }

    private void Init()
    {
      topRowIndex = 0;
      CreateComponents();
      SetScrollBarToCurrentIndex();
      populateClickableComponentList();
    }

    private void CreateComponents()
    {
      moveUpButtons.Clear();
      moveDownButtons.Clear();
      visibilityButtons.Clear();
      renameButtons.Clear();
      rows.Clear();

      for (int i = 0; i < warpObjects.Count; i++)
      {
        var row = new ClickableComponent(
          new Rectangle(xPositionOnScreen + 16, yPositionOnScreen + 16 + (i - topRowIndex) * ((height - 32) / pageSize), width - 32, (height - 32) / pageSize + 4),
          string.Concat(i)
        )
        {
          myID = i,
          downNeighborID = i + 1,
          upNeighborID = i > 0 ? i - 1 : -1,
          rightNeighborID = i + hideButtonOffset,
          leftNeighborID = i < warpObjects.Count - 1 ? i + moveDownButtonOffset : i + moveUpButtonOffset,
          fullyImmutable = true
        };

        var moveUpButton = new MoveUpButton(i, this, skip: i == 0);
        moveUpButton.SetupIDs(
          ID: i > 0 ? i + moveUpButtonOffset : -7777,
          upID: i > 0 ? i - 1 + moveDownButtonOffset : -1,
          downID: i + moveDownButtonOffset,
          leftID: -7777,
          rightID: i
        );

        var moveDownButton = new MoveDownButton(i, this, skip: i == warpObjects.Count - 1);
        moveDownButton.SetupIDs(
          ID: i < warpObjects.Count - 1 ? i + moveDownButtonOffset : -7777,
          upID: i + moveUpButtonOffset,
          downID: i + 1 + moveUpButtonOffset,
          leftID: -7777,
          rightID: i
        );

        var visibilityButton = new VisibilityButton(i, warpObjects[i].Hidden, this, warpObjects[i].WarpDoWhen.Do.Type == WarpDoType.Farm);
        visibilityButton.SetupIDs(
          ID: i + hideButtonOffset,
          upID: i > 0 ? i - 1 + hideButtonOffset : -1,
          downID: i + 1 + hideButtonOffset,
          leftID: i,
          rightID: i + renameButtonOffset
        );

        var renameButton = new RenameButton(i, this);
        renameButton.SetupIDs(
          ID: i + renameButtonOffset,
          upID: i > 0 ? i - 1 + renameButtonOffset : upperRightCloseButton_ID,
          downID: i + 1 + renameButtonOffset,
          leftID: i + hideButtonOffset,
          rightID: i > 0 ? -7777 : upperRightCloseButton_ID
        );

        moveUpButtons.Add(moveUpButton);
        moveDownButtons.Add(moveDownButton);
        visibilityButtons.Add(visibilityButton);
        renameButtons.Add(renameButton);
        rows.Add(row);
      }

      scrollUpButton = new ClickableTextureComponent(
        new Rectangle(xPositionOnScreen + width + 16, yPositionOnScreen, 44, 48),
        Game1.mouseCursors,
        new Rectangle(421, 459, 11, 12),
        4f
        );
      scrollDownButton = new ClickableTextureComponent(
        new Rectangle(xPositionOnScreen + width + 16, yPositionOnScreen + height - 64, 44, 48),
        Game1.mouseCursors,
        new Rectangle(421, 472, 11, 12),
        4f
      );
      scrollBar = new ClickableTextureComponent(
        new Rectangle(scrollUpButton.bounds.X + 12, scrollUpButton.bounds.Y + scrollUpButton.bounds.Height + 4, 24, 40),
        Game1.mouseCursors,
        new Rectangle(435, 463, 6, 10),
        4f
      );
      scrollBarRunner = new Rectangle(
        scrollBar.bounds.X,
        scrollUpButton.bounds.Y + scrollUpButton.bounds.Height + 4,
        scrollBar.bounds.Width,
        height - 64 - scrollUpButton.bounds.Height - 8
      );

      initializeUpperRightCloseButton();
      upperRightCloseButton.myID = upperRightCloseButton_ID;
    }

    private void SetScrollBarToCurrentIndex()
    {
      if (rows.Count > 0)
      {
        scrollBar.bounds.Y = scrollBarRunner.Height / Math.Max(1, rows.Count - pageSize + 1) * topRowIndex + scrollUpButton.bounds.Bottom + 4;
        if (topRowIndex == rows.Count - pageSize)
        {
          scrollBar.bounds.Y = scrollDownButton.bounds.Y - scrollBar.bounds.Height - 4;
        }
      }

      UpdateRowsPositions();
    }

    private void UpdateRowsPositions()
    {
      for (int i = 0; i < rows.Count; i++)
      {
        var row = rows[i];
        var y = yPositionOnScreen + 16 + (i - topRowIndex) * ((height - 32) / pageSize);
        row.bounds.Y = y;

        moveUpButtons[i].UpdatePosition(row.bounds.Left + 12, row.bounds.Y + 12);
        moveDownButtons[i].UpdatePosition(row.bounds.Left + 12, row.bounds.Y + row.bounds.Height / 2);
        visibilityButtons[i].UpdatePosition(row.bounds.Right - row.bounds.Width / 12 * 2 - 12, row.bounds.Y + row.bounds.Height / 4 + 8);
        renameButtons[i].UpdatePosition(row.bounds.Right - row.bounds.Width / 12 - 12, row.bounds.Y + row.bounds.Height / 4);
      }
    }

    private void UpArrowPressed()
    {
      topRowIndex--;
      scrollUpButton.scale = 3.5f;
      SetScrollBarToCurrentIndex();
    }

    private void DownArrowPressed()
    {
      topRowIndex++;
      scrollDownButton.scale = 3.5f;
      SetScrollBarToCurrentIndex();
    }

    public void ConstrainSelectionToVisibleSlots()
    {
      if (rows.Contains(currentlySnappedComponent))
      {
        int index = allClickableComponents.IndexOf(currentlySnappedComponent) % rows.Count;
        if (index < topRowIndex)
        {
          index = topRowIndex;
        }
        else if (index >= topRowIndex + pageSize)
        {
          index = topRowIndex + pageSize - 1;
        }

        currentlySnappedComponent = rows[index];
        if (Game1.options.snappyMenus && Game1.options.gamepadControls)
        {
          snapCursorToCurrentSnappedComponent();
        }
      }
    }

    public override void applyMovementKey(int direction)
    {
      base.applyMovementKey(direction);
      if (allClickableComponents.Contains(currentlySnappedComponent))
      {
        int index = allClickableComponents.IndexOf(currentlySnappedComponent) % rows.Count;
        if (index < topRowIndex)
        {
          topRowIndex = index;
        }
        else if (index >= topRowIndex + pageSize)
        {
          topRowIndex = index - pageSize + 1;
        }

        SetScrollBarToCurrentIndex();
        UpdateRowsPositions();

        if (Game1.options.snappyMenus && Game1.options.gamepadControls)
        {
          snapCursorToCurrentSnappedComponent();
        }
      }
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (scrollUpButton.containsPoint(x, y) && topRowIndex > 0)
      {
        UpArrowPressed();
        Game1.playSound("shwip");
        return;
      }

      if (scrollDownButton.containsPoint(x, y) && topRowIndex < rows.Count - pageSize)
      {
        DownArrowPressed();
        Game1.playSound("shwip");
        return;
      }

      if (scrollBar.containsPoint(x, y))
      {
        scrolling = true;
        return;
      }

      if (!scrollDownButton.containsPoint(x, y) && x > xPositionOnScreen + width && x < xPositionOnScreen + width + 128 && y > yPositionOnScreen && y < yPositionOnScreen + height)
      {
        scrolling = true;
        leftClickHeld(x, y);
        releaseLeftClick(x, y);
        return;
      }

      for (int i = topRowIndex; i < topRowIndex + pageSize && i < rows.Count; i++)
      {
        moveUpButtons[i].receiveLeftClick(x, y, playSound);
        moveDownButtons[i].receiveLeftClick(x, y, playSound);
        visibilityButtons[i].receiveLeftClick(x, y, playSound);
        renameButtons[i].receiveLeftClick(x, y, playSound);
      }

      topRowIndex = Math.Max(0, Math.Min(rows.Count - pageSize, topRowIndex));

      base.receiveLeftClick(x, y, playSound);
    }

    public override void leftClickHeld(int x, int y)
    {
      base.leftClickHeld(x, y);
      if (scrolling)
      {
        int y2 = scrollBar.bounds.Y;
        scrollBar.bounds.Y = Math.Min(yPositionOnScreen + height - 64 - 12 - scrollBar.bounds.Height, Math.Max(y, yPositionOnScreen + scrollUpButton.bounds.Height + 20));
        float num = (float)(y - scrollBarRunner.Y) / (float)scrollBarRunner.Height;
        topRowIndex = Math.Min(rows.Count - pageSize, Math.Max(0, (int)((float)rows.Count * num)));
        SetScrollBarToCurrentIndex();
        if (y2 != scrollBar.bounds.Y)
        {
          Game1.playSound("shiny4");
        }
      }
    }

    public override void performHoverAction(int x, int y)
    {
      base.performHoverAction(x, y);

      if (rows.Count > 0)
      {
        scrollUpButton.tryHover(x, y);
        scrollDownButton.tryHover(x, y);
      }

      for (int i = topRowIndex; i < topRowIndex + pageSize && i < rows.Count; i++)
      {
        moveUpButtons[i].performHoverAction(x, y);
        moveDownButtons[i].performHoverAction(x, y);
        visibilityButtons[i].performHoverAction(x, y);
        renameButtons[i].performHoverAction(x, y);
      }
    }

    public override void receiveScrollWheelAction(int direction)
    {
      base.receiveScrollWheelAction(direction);
      if (direction > 0 && topRowIndex > 0)
      {
        UpArrowPressed();
        ConstrainSelectionToVisibleSlots();
        Game1.playSound("shiny4");
      }
      else if (direction < 0 && topRowIndex < Math.Max(0, rows.Count - pageSize))
      {
        DownArrowPressed();
        ConstrainSelectionToVisibleSlots();
        Game1.playSound("shiny4");
      }
    }

    public override void populateClickableComponentList()
    {
      allClickableComponents ??= new();
      allClickableComponents.Clear();
      allClickableComponents.AddRange(rows);
      allClickableComponents.AddRange(moveUpButtons.Select(b => b.ClickableComponent));
      allClickableComponents.AddRange(moveDownButtons.Select(b => b.ClickableComponent));
      allClickableComponents.AddRange(visibilityButtons.Select(b => b.ClickableComponent));
      allClickableComponents.AddRange(renameButtons.Select(b => b.ClickableComponent));
      allClickableComponents.Add(upperRightCloseButton);
    }

    public override void snapToDefaultClickableComponent()
    {
      if (topRowIndex < rows.Count)
      {
        currentlySnappedComponent = rows[topRowIndex];
      }

      base.snapCursorToCurrentSnappedComponent();
    }

    public override void draw(SpriteBatch b)
    {
      var color = Color.White;

      // draw faded background
      b.Draw(
        Game1.fadeToBlackRect,
        Game1.graphics.GraphicsDevice.Viewport.Bounds,
        Color.Black * 0.75f
      );
      // draw menu title
      SpriteText.drawStringWithScrollCenteredAt(
        b,
        TranslatedKeys.Configuration,
        xPositionOnScreen + width / 2,
        yPositionOnScreen - 64
      );
      // draw menu box
      drawTextureBox(
        b,
        Game1.mouseCursors,
        new Rectangle(384, 373, 18, 18),
        xPositionOnScreen,
        yPositionOnScreen,
        width,
        height,
        Color.White,
        4f
      );
      hoverText = string.Empty;

      if (topRowIndex < 0) topRowIndex = 0;
      for (int i = topRowIndex; i < topRowIndex + pageSize && i < rows.Count; i++)
      {
        var row = rows[i];
        // draw row box
        drawTextureBox(
          b,
          Game1.mouseCursors,
          new Rectangle(384, 396, 15, 15),
          row.bounds.X,
          row.bounds.Y,
          row.bounds.Width,
          row.bounds.Height,
          color,
          4f,
          drawShadow: false
        );

        // draw image
        drawTextureBox(
          b,
          spritesheetTexture,
          warpObjects[i].SpirteSource,
          row.bounds.X + 48,
          row.bounds.Y + 16,
          64,
          64,
          color * 0.9f,
          1f,
          false
        );

        // draw name
        SpriteText.drawString(
          b,
          warpObjects[i].Text,
          row.bounds.X + 128,
          row.bounds.Y + 24
        );
      }

      // draw scrollbar
      if (rows.Count > pageSize)
      {
        scrollUpButton.draw(b);
        scrollDownButton.draw(b);
        drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), scrollBarRunner.X, scrollBarRunner.Y, scrollBarRunner.Width, scrollBarRunner.Height, Color.White, 4f);
        scrollBar.draw(b);
      }

      // draw buttons
      for (int i = topRowIndex; i < topRowIndex + pageSize && i < rows.Count; i++)
      {
        moveUpButtons[i].draw(b);
        moveDownButtons[i].draw(b);
        visibilityButtons[i].draw(b);
        renameButtons[i].draw(b);

        if (moveUpButtons[i].Hovered) hoverText = moveUpButtons[i].HoverText;
        if (moveDownButtons[i].Hovered) hoverText = moveDownButtons[i].HoverText;
        if (visibilityButtons[i].Hovered) hoverText = visibilityButtons[i].HoverText;
        if (renameButtons[i].Hovered) hoverText = renameButtons[i].HoverText;
      }

      // draw hover text
      if (!hoverText.IsEmpty()) drawHoverText(b, hoverText, Game1.smallFont);

      base.draw(b);
      drawMouse(b);
    }
  }
}