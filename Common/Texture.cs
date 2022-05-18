using OpenTK.Graphics.OpenGL4;
using System.Drawing;
using System.Drawing.Imaging;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace LearnOpenTK.Common
{
    public class Texture //класс для загрузки текстур
    {
        public readonly int Handle;

        public static Texture LoadFromFile(string path)
        {
            /*Загрузка текстуры*/
            int handle = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, handle);

            using (var image = new Bitmap(path)) //загрузии изображение
            {
                // Bitmap загружается из верхнего левого пикселя (а OpenGL загружается из нижнего левого пикселя, в результате чего текстура переворачивается по вертикали) -> переворачиваем картинку
                image.RotateFlip(RotateFlipType.RotateNoneFlipY); 

                var data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb); //получаем пиксели из загруженного  изображения
                // Аргументы: 1ый - требуемая площадь в пикселях (от (0,0) до (ширина, высота) ), 2ой - что делаем с пикселями, 3ий -формат пикселей, в котором хотели бы хранить

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height,  0, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            }
            /*END (текстура загружена)*/

            /*Генерация MipMap - уменьшенные копии текстуры в уменьшенном масштабе.*/
            //Каждый уровень MIP-карты в два раза меньше предыдущего.
            //Сгенерированные мип-карты уменьшаются до одного пикселя.
            // OpenGL будет автоматически переключаться между MIP-картами, когда объект окажется достаточно далеко.
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            /*END*/

            return new Texture(handle);
        }

        public Texture(int glHandle)
        {
            Handle = glHandle;
        }
        public void Use(TextureUnit unit)
        {
            GL.ActiveTexture(unit);                                 //активируем текстуру
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }
    }
}
