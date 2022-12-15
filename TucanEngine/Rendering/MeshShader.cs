using TucanEngine.Rendering.Tools;

namespace TucanEngine.Rendering
{
    public class MeshShader : ShaderProgram
    {
        private static MeshShader current;
        private const string VertexShaderCode = @"
#version 150

in vec3 position;
in vec2 textureCoordinates;

out vec3 colour;
out vec2 pass_textureCoordinates;

uniform mat4 modelMatrix;
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;

void main(void){

	gl_Position = projectionMatrix * viewMatrix * modelMatrix * vec4(position,1.0);
	pass_textureCoordinates = textureCoordinates;
}";

        private const string FragmentShaderCode = @"
#version 150

in vec3 colour;
in vec2 pass_textureCoordinates;

out vec4 out_Color;

uniform sampler2D modelTexture;

void main(void){
	out_Color = texture(modelTexture,pass_textureCoordinates);
}";

        public MeshShader() : base(VertexShaderCode, FragmentShaderCode) {
            current = this;
        }

        public static MeshShader GetCurrentShader() {
            return current;
        }

        protected override void BindAttributes() {
            BindAttribute(0, "position");
            BindAttribute(1, "textureCoordinates");
        }
    }
}