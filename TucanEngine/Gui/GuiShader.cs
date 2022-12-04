using TucanEngine.Rendering.Tools;

namespace TucanEngine.Gui
{
    public class GuiShader : ShaderProgram
    {
        private const string VertexShaderCode = @"
#version 150

in vec2 position;
in vec2 textureCoordinates;

uniform mat4 modelMatrix;

out vec2 pass_textureCoordinates;

void main(void) {
	gl_Position = modelMatrix * vec4(position, 0.0, 1.0);
	pass_textureCoordinates = textureCoordinates;
}";

        private const string FragmentShaderCode = @"
#version 150

in vec2 pass_textureCoordinates;

out vec4 out_Color;

uniform sampler2D imageTexture;
uniform vec2 dimensions;
uniform bool isStretched;
uniform vec4 color;

const float textureBorderDensity = 0.25;
const float windowBorderDensity = 0.03;

float map(float value, float min1, float max1, float min2, float max2) {
    return min2 + (value - min1) * (max2 - min2) / (max1 - min1);
}

float negate(float value){
    return 1 - value;
}

float stretch(float coord, float textureBorder, float windowBorder) {
    if (coord < windowBorder)
        return map(coord, 0, windowBorder, 0, textureBorder) ;

    float negTextureBorder = negate(textureBorder);
    float negWindowBorder = negate(windowBorder);

    if (coord < negWindowBorder) 
        return map(coord,  windowBorder, negWindowBorder, textureBorder, negTextureBorder);

    return map(coord, negWindowBorder, 1, negTextureBorder, 1);
} 

void main(void) {
    vec2 borders = vec2(windowBorderDensity) / dimensions;
    vec2 processedTextureCoordinates = pass_textureCoordinates;

    if(isStretched)
        processedTextureCoordinates = vec2(
          stretch(processedTextureCoordinates.x, textureBorderDensity, borders.x),
          stretch(processedTextureCoordinates.y, textureBorderDensity, borders.y));

	out_Color = texture(imageTexture, processedTextureCoordinates) * color;

    if(out_Color.a < 0.5) discard;
}";

        public GuiShader() : base(VertexShaderCode, FragmentShaderCode) { }

        protected override void BindAttributes() {
            BindAttribute(0, "position");
            BindAttribute(1, "textureCoordinates");
        }
    }
}