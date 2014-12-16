using System;
using Tao.FreeGlut;
using OpenGL;

namespace OpenGLCSharp{
    class Program{
        //nastaveni okenka
        private static int width = 1366;
        private static int height = 768;
        private static int pos_x = 0;
        private static int pos_y = 0;
        private static string window_Title = "Stepanovo okenko do budoucnosti";

        //nastaveni Shaderu
        private static ShaderProgram SHprogram;

        //Objects
        private static VBO<Vector3> triangle;
        private static VBO<Vector3> triangle_col;
        private static VBO<int> triangle_el;

        //Bonus
        private static System.Diagnostics.Stopwatch watch;
        private static float angle;

        static void Main(string[] args)    {
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_DEPTH | Glut.GLUT_DOUBLE);
            Glut.glutInitWindowSize(width, height);
            Glut.glutInitWindowPosition(pos_x, pos_y);
            Glut.glutCreateWindow(window_Title);
            Glut.glutIdleFunc(onRenderFrame);
            Glut.glutDisplayFunc(onDisplay);
            
            SHprogram = new ShaderProgram(VertexShader, FragmentShader);
            SHprogram.Use();
            SHprogram["projection_matrix"].SetValue(Matrix4.CreatePerspectiveFieldOfView(0.45f, (float)width / height, 0.1f, 10000f));
            SHprogram["view_matrix"].SetValue(Matrix4.LookAt(new Vector3(0, 0, 10), Vector3.Zero, Vector3.Up));

            triangle = new VBO<Vector3>(new Vector3[] {new Vector3(-1, -1, 0), new Vector3(1, -1, 0), new Vector3(0, 1, 0) });
            triangle_el = new VBO<int>(new int[] { 0, 1, 2 }, BufferTarget.ElementArrayBuffer);
            triangle_col = new VBO<Vector3>(new Vector3[] { new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 1) });

            watch = System.Diagnostics.Stopwatch.StartNew();

            Glut.glutMainLoop();
        }
        private static void onRenderFrame(){

            watch.Stop();
            float delat = (float)watch.ElapsedMilliseconds / System.Diagnostics.Stopwatch.Frequency;
            watch.Restart();
            angle += delat * 10000;

            Gl.Viewport(pos_x, pos_y, width, height);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            SHprogram.Use();

            SHprogram["model_matrix"].SetValue(Matrix4.CreateRotationY(angle) * Matrix4.CreateTranslation(new Vector3(-1.5f, 0, 0)));
            uint vertexPos = (uint)Gl.GetAttribLocation(SHprogram.ProgramID, "vertexPos");
            Gl.EnableVertexAttribArray(vertexPos);
            Gl.BindBuffer(triangle);
            Gl.VertexAttribPointer(vertexPos, triangle.Size, triangle.PointerType, true, 12, IntPtr.Zero);
            Gl.BindBufferToShaderAttribute(triangle_col, SHprogram, "vertexCol");
            Gl.BindBuffer(triangle_el);
            Gl.DrawElements(BeginMode.Triangles, triangle_el.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);

            Glut.glutSwapBuffers();

        }
        private static void onDisplay(){

        }



        
        //Toto bude rozhodne nutne ZMENIT!
        private static string VertexShader = @"
            in vec3 vertexCol;
            in vec3 vertexPos;
            
            out vec3 Color;

            uniform mat4 projection_matrix;
            uniform mat4 view_matrix;
            uniform mat4 model_matrix;
            void main(void){
                Color = vertexCol;
                gl_Position = projection_matrix*view_matrix*model_matrix*vec4(vertexPos, 1);

            }
            ";
        private static string FragmentShader = @"
            in vec3 Color;
    
            void main(void){
                gl_FragColor = vec4(Color, 1);
            }
        ";

    }
}
