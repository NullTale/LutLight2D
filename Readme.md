# LutLight2D
Dev by NullTale<br>
[![Itch](https://img.shields.io/badge/Web-Itch?logo=Itch.io&color=white)](https://nulltale.itch.io)
[![Twitter](https://img.shields.io/badge/Twitter-Twitter?logo=X&color=red)](https://twitter.com/NullTale)
[![Discord](https://img.shields.io/badge/Discord-Discord?logo=discord&color=white)](https://discord.gg/CkdQvtA5un)
[![Boosty](https://img.shields.io/badge/Support-Boosty?logo=boosty&color=white)](https://boosty.to/nulltale/donate)

** Just a Unique lighting system that has no analogies ¯\\(ツ)/¯

• Stylized pixel art lighting via color replacement.<br>
It keeps the original colors of the palette and can create unusual stylization effects.

• Web Demo https://nulltale.itch.io/dungeon-tale

• Tested with Unity 2021, 2022, uses Urp 2D Renderer and Shader Graph.

[![Asset Store](https://img.shields.io/badge/Asset%20Store-asd?logo=Unity&color=red)](https://assetstore.unity.com/packages/tools/particles-effects/lutlight2d-267033)
[![Forum](https://img.shields.io/badge/Forum-asd?logo=ChatBot&color=blue)](https://forum.unity.com/threads/1542449/)
<!---
([![Aseprite Extension](https://img.shields.io/badge/Aseprite%20Plugin-asd?logo=Aseprite&color=white)](https://github.com/zacholade/lut-light-aseprite-extension))
> Thanks to [Zacholade](https://github.com/zacholade) for Aseprite [Extension](https://github.com/zacholade/lut-light-aseprite-extension) to preview changes in the editor
--->

![_cover](https://github.com/NullTale/LutLight2D/assets/1497430/871a332c-0cec-4811-93d5-d8a804525d02)<br>
<img src="https://user-images.githubusercontent.com/1497430/229509448-da8a1939-4371-4938-8e6c-642c95c71697.gif" width="600"><br>

## • Examples

<sub>Pattern shading [Oil6](https://lospec.com/palette-list/oil-6)</sub><br>
<img src="https://user-images.githubusercontent.com/1497430/229741056-ee26e56c-57c0-42fe-89e5-db16ba03ff0e.gif" width="600"><br>

<sub>Sharp gradient shading [Fantasy 24](https://lospec.com/palette-list/fantasy-24)</sub><br>
<img src="https://user-images.githubusercontent.com/1497430/229741062-c004c67c-6d4f-4870-9550-5565235c2924.gif" width="600"><br>
 <a name="elospec500"></a>
<sub>In shadow visible objects [Lospec500](https://lospec.com/palette-list/lospec500)</sub><br>
<img src="https://user-images.githubusercontent.com/1497430/229741049-6afbb859-b664-4bc1-bf3f-9fc4ec8bb121.gif" width="600"><br>

<sub>In shadow outline [FUZZYFOUR](https://lospec.com/palette-list/fuzzyfour)</sub><br>
<img src="https://user-images.githubusercontent.com/1497430/229741007-9052d285-5bcf-4a07-8e24-f1fbef4aea79.gif" width="600"><br>

## • How it works
The general idea is to make each color have its own shading gradient defined manually<br>
like it is done in most pixel art palettes.

<img src="https://user-images.githubusercontent.com/1497430/229498868-01bfcdb4-0ca4-40c8-a186-2b3af9366f1b.png" width="600"><br>

The shader does this using a set of [Lut](https://lettier.github.io/3d-game-shaders-for-beginners/lookup-table.html) tables generated from gradient ramps.<br>
The color for replacement is taken from the tables depending on the brightness from the lighting texture.

<img src="https://user-images.githubusercontent.com/1497430/229542912-c903c884-b08d-4772-bbee-3d2bd5a323aa.png" width="600"><br>

Colors from the tables can be taken with interpolation blending, to make smooth transitions.<br>
By specify color ramps and using material options, effects as in the [examples](#examples) can be achieved.

<img src="https://user-images.githubusercontent.com/1497430/229517322-98e91e47-0c54-47f0-8f0a-a10487153583.png" width="500"><br>

## • Installation and use

Install via Unity [PackageManager](https://docs.unity3d.com/Manual/upm-ui-giturl.html)<br>
```
https://github.com/NullTale/LutLight2D.git
```

<img src="https://user-images.githubusercontent.com/1497430/213906801-7cab3334-5626-46b8-9966-d5c0b6107edc.png">

> Bu default the Shader uses the lighting texture from the `Urp 2D Renderer` so `Urp Asset` must be configured.

First needed to create the `LutLight2D Asset`, it will generate a texture with a set of lut tables for the material.

<img src="https://user-images.githubusercontent.com/1497430/229541874-3e9ebf82-ef78-4597-bf94-60e731550475.png" width="500"><br>

It will create a material and Grayscale Ramps for it, which should already be enough to apply the material.

<img src="https://user-images.githubusercontent.com/1497430/229521544-3bec1376-7c8f-4bb4-809d-ab106f638af5.png" width="600"><br>

Next step is to set the gradient table for each color from the palette.<br>
After the gradients are set, need to press the Bake button to apply the result if it was not done automatically.<br>

<sup>Result with the character sprite in the [lospec500](https://lospec.com/palette-list/lospec500) palette.</sup><br>
<img src="https://user-images.githubusercontent.com/1497430/230003982-d3a49fcd-8b62-44d3-b6b8-feb16b60d7d7.png" width="600"><br>

The Ramps file is a common .png which contains shading gradient for each color from the palette, starting from the original, lightest, to the darkest.<br>
> Colors also can have alpha channel.

<img src="https://github.com/NullTale/LutLight2D/assets/1497430/6311a672-8e0c-418d-9c42-5328af745f84" width="450"><br>

Now the character shaded with black color from the palette, and his light areas like eyes and face are visible in the shadows.<br>
By changing the Color Ramps, unusual materials can be made that are only visible in shadow or light, as in the [example](#elospec500) for the lospec500 palette.<br>

> Examples with other applications can be found in the package samples.

---
## Misc

### • What this really is ?

The name Pixel Art lighting is chosen for clickbait and to get search engine traffic.<br>
In general the system and approach itself is something more, it is an artistic lighting with dynamic gradients through color replacement that can be used much more flexibly in both 2D and 3D to express artistic practices in real time.<br>
Though simplified and developed in the direction of PixelArt, the Asset itself is inspired by materials on working with color from painting and applied to artistic purposes.

• Below is a semi-entertaining video about choosing colors using gradients and different ways of transitioning through the color wheel, one of which was inspired by this approuch.<br>
[![IMAGE ALT TEXT](http://img.youtube.com/vi/OdoW9qgwPf4/0.jpg)](https://youtu.be/OdoW9qgwPf4?t=38 "Video Title")

• Math video about Phong Lighting in contrast with previous one, basically, not very good linear gradient from white to black that was applied as multiplyer over the main image. (common approuch in lighting)<br>
[![IMAGE ALT TEXT](http://img.youtube.com/vi/LKXAIuCaKAQ/0.jpg)](https://www.youtube.com/watch?v=LKXAIuCaKAQ "Video Title")

### • Zacholade !

Unfinished, NOT Documentated [![Aseprite Extension](https://img.shields.io/badge/Aseprite%20Extension-asd?logo=Aseprite&color=white)](https://github.com/zacholade/lut-light-aseprite-extension) 
from [Zacholade](https://github.com/zacholade) to preview changes in the editor<br>
(originally was added in to the description but later removed because of it's state (that was allowed by it's autor!!))

> people who are not indifferent, please give him Issues to stimulate a development

https://github.com/user-attachments/assets/080b548b-ecef-4cc9-9d1b-7f65afb992bf
