using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Imaging.Filters;
using AForge;
using Segmentation;
using FeatureExtraction;
using System.IO;
using SVM;

namespace sample
{
    public partial class Form1 : Form
  
    {

        Bitmap originalImage;
        Bitmap segmentImage;

        String folderPath;
        Model model;

        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                originalImage = new Bitmap(openDialog.FileName);
                pictureBox1.Image = originalImage;
               
            }
        }


        private void FeatureExtraction_Click(object sender, EventArgs e)

        {
            HandSegmentation handSegObject = new HandSegmentation();
            //the line used to connect HandSegmentation with form1 which inturn creates .dll file
           segmentImage = handSegObject.Apply((Bitmap)originalImage.Clone());
           

            HOEF hoefObject = new HOEF();
            float[] featureVector =
                hoefObject.Apply((Bitmap)segmentImage.Clone());
        }

        private void WriteToFile(List<float[]> features,int label ,ref StreamWriter sw)
        {
           

            foreach (var featureVector in features)
            {
                sw.Write(label + " ");
                for (int index = 0; index < featureVector.Length; index++)
                {
                    sw.Write((index + 1) + ":" + featureVector[index] + " ");
                }
                sw.WriteLine();
            }

           

            sw.Flush();
            
        }


        private void featureExtraction_Click(object sender, EventArgs e) //button2
        {

            HandSegmentation segObj = new HandSegmentation();
            HOEF hoefObj = new HOEF();

            FileStream fs =
                new FileStream("Train", FileMode.Create ,FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);


           
            for (int i = 0; i < 2; i++)
            {
                List<float[]> features = new List<float[]>();
                char subFolder = (char)('a' + i);
                String finalFolder = folderPath + "\\" + subFolder;

                String[] allfiles = Directory.GetFiles(finalFolder);

                for (int index = 0; index < allfiles.Length; index++)
                {
                    try
                    {
                        Bitmap img = new Bitmap(allfiles[index]);
                        Bitmap segmentImage = segObj.Apply((Bitmap)img.Clone());
                        float[] featureVector = hoefObj.Apply((Bitmap)segmentImage.Clone());
                        features.Add(featureVector);
                    }
                    catch
                    {

                    }
                }
                WriteToFile(features,i,ref sw);
                
            }

            sw.Close();

            fs.Close();
            MessageBox.Show("writing to file is done");

        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
           if( folderDialog.ShowDialog() == DialogResult.OK)
            {
                folderPath = folderDialog.SelectedPath;
            }
           
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Problem train = Problem.Read("Train");
            Parameter parameters = new Parameter();
            parameters.C = 32; parameters.Gamma = 8;
            model = Training.Train(train, parameters);
            MessageBox.Show("model is trained");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            if(openDialog.ShowDialog() == DialogResult.OK)
            {
                Bitmap img = new Bitmap(openDialog.FileName);
                pictureBox1.Image = img;

                HandSegmentation segObj = new HandSegmentation();
                Bitmap segImage = segObj.Apply((Bitmap)img.Clone());

                HOEF hoefObj = new HOEF();
                float[] featureVector = hoefObj.Apply(segImage);

                List<float[]> features = new List<float[]>();
                features.Add(featureVector);

                FileStream fs = new FileStream("Test", FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);

                WriteToFile(features, 40, ref sw);

                sw.Flush();
                sw.Close();
                fs.Close();

                Problem test = Problem.Read("Test");
                Prediction.Predict(test, "result", model, false);

                FileStream fsRead = new FileStream("result", FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fsRead);
                string result = sr.ReadLine();

                sr.Close();
                fsRead.Close();

                int iResult = Int32.Parse(result);

                char[] lookuptable = { 'A', 'B', 'C', 'D' };
                char output = lookuptable[iResult];
                label1.Text = output.ToString();
            }
        }

       

        
    }
}
