# LutLight2D

Stylized pixel art lighting shader for Unity2D using a color replacement technique.<br>
It keeps the original colors of the palette and can create unusual stylization effects.


![LutLight_Compare](https://user-images.githubusercontent.com/1497430/229509448-da8a1939-4371-4938-8e6c-642c95c71697.gif)

### Examples

<sub>In shadow visible objects [Lospec500](https://lospec.com/palette-list/lospec500)</sub><br>
![LutLight_Example_A](https://user-images.githubusercontent.com/1497430/229535181-3bdb4d3b-e157-4946-8748-9f64511265da.gif)

<sub>Pattern shading [Oil6](https://lospec.com/palette-list/oil-6)</sub><br>
![LutLight_Example_B](https://user-images.githubusercontent.com/1497430/229535216-328d89e1-ddda-4dd5-8ad6-f49eb8177315.gif)

<sub>Just sharp gradient shading [Fantasy 24](https://lospec.com/palette-list/fantasy-24)</sub><br>
![LutLight_Example_C](https://user-images.githubusercontent.com/1497430/229535227-e373449a-e778-4823-8ebf-f5b0f80176d9.gif)

<sub>In shadow outline [FUZZYFOUR](https://lospec.com/palette-list/fuzzyfour)</sub><br>
![LutLight_Example_D](https://user-images.githubusercontent.com/1497430/229535237-265e34a3-447e-44b2-abbb-ec59f7f12861.gif)

## How it works
The general idea is to make each color have its own shading gradient defined manually<br>
like it is done in most pixel art palettes.

<sup>[Fantasy 24](https://lospec.com/palette-list/fantasy-24) palette from [Gabriel C](https://twitter.com/_universs) </sup> <br>
<img src="https://user-images.githubusercontent.com/1497430/229498868-01bfcdb4-0ca4-40c8-a186-2b3af9366f1b.png" width="600"><br>

The shader does this using a set of [Lut](https://lettier.github.io/3d-game-shaders-for-beginners/lookup-table.html) tables generated from gradient ramps.

![LutTables](https://user-images.githubusercontent.com/1497430/229542912-c903c884-b08d-4772-bbee-3d2bd5a323aa.png)<br>
<sup>The color for replacement is taken from the tables depending on the brightness from the lighting texture.</sup>

Colors from the tables can be taken with interpolation blending, to make smooth transitions.<br>
By specify color ramps and using material options, effects as in the [examples](#examples) can be achieved.

![image](https://user-images.githubusercontent.com/1497430/229517322-98e91e47-0c54-47f0-8f0a-a10487153583.png)<br> 

## Installation and use

Install via PackageManager `https://github.com/NullTale/LutLight2D.git` <br>
<img src="https://user-images.githubusercontent.com/1497430/213906801-7cab3334-5626-46b8-9966-d5c0b6107edc.png">

> The Shader uses the lighting texture from the Urp `2D Renderer` so `Urp Asset` must be configured.

First needed to create the `LutLight2D Asset`, it will generate a texture with a set of lut tables for the material.

<img src="https://user-images.githubusercontent.com/1497430/229541874-3e9ebf82-ef78-4597-bf94-60e731550475.png" width="600"><br>

It will create material and Grayscale Ramps to it, which should already be enough to apply material.

<img src="https://user-images.githubusercontent.com/1497430/229521544-3bec1376-7c8f-4bb4-809d-ab106f638af5.png" width="600"><br>

Next step is to set the gradient table for each color from the palette.<br>
After the gradients are set, need to press the Bake button to apply the result if it was not done automatically.
> The Ramps file is a common .png which contains shading gradient for each color from the palette, starting from the original, lightest, to the darkest color.
> Colors also can alpha chanel.

<sup>Result with the character sprite in the [lospec500](https://lospec.com/palette-list/lospec500) palette.</sup><br>
<img src="https://user-images.githubusercontent.com/1497430/229498871-8c0615b5-bea2-4158-b7e5-e2f42f903441.png" width="600"><br>

