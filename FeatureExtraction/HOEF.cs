using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace FeatureExtraction
{
    public class HOEF
    {
       public float [] Apply(Bitmap segmentImage)
        {
            var copiedImage = (Bitmap)segmentImage.Clone();

            int N = 6;
            int M = 6;
            int count;
            int wholeImageCount = 0;

            List<float> featureVector = new List<float>();

            int width = copiedImage.Width / N;
            int height = copiedImage.Height / M;

            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    count = 0;
                    for (int k = i * width; k < (i + 1) * width; k++)
                    {
                        for (int l = j * height; l < (j + 1) * height; l++)
                        {
                            Color c = copiedImage.GetPixel(k, 1);
                            if (c.R == 255 && c.G == 255 && c.B == 255)
                            {
                                count++;
                            }
                        }
                    }
                    wholeImageCount += count;
                    featureVector.Add(count);
                }
            }
            for (int i = 0; i < featureVector.Count; i++)
            {
                featureVector[i] /= wholeImageCount;
            }

            return featureVector.ToArray();
        }
    
    }
}
