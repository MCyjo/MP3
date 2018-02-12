using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio;
using NAudio.Wave;
using YoutubeExtractor;
using System.Threading;

namespace WindowsFormsApp3
{
    public delegate void PlayTrack();

    public partial class Form1 : Form
    {
        xmlHandler xml;
        PlayTrack playTrack;
        IWavePlayer waveOutDevice;
        AudioFileReader audioFileReader;
        bool isPlaying=false;
        int trackTime;
        int trackNumber;
        Random randomNumber = new Random();
        List<string> paths = new List<string>();
        string[] fileName;

        public Form1()
        {
            xml=new xmlHandler(paths, this);
            playTrack = play;
            InitializeComponent();
            waveOutDevice = new WaveOut();
            
        }

        public void play()
        {
            try
            {
                audioFileReader = new AudioFileReader(paths[trackNumber]);
                waveOutDevice.Init(audioFileReader);
                trackTime = (int)audioFileReader.TotalTime.TotalSeconds;
                trackBar1.Maximum = trackTime;

                listBox1.SelectedIndex = trackNumber;
                waveOutDevice.Play();
                string Name = (string)listBox1.SelectedItem;
                fileName = Name.Split('\\');
                textBox1.Text = fileName[fileName.Count()-1];
            }
               catch
            {
                MessageBox.Show("Dodaj utwór do playlisty!");
            }  
        }

        public void playRandom()
        {
            
            trackNumber = (randomNumber.Next()-1 ) %(paths.Count());
            play();
        }

        private void button1_Click(object sender, EventArgs e)  //play button
        {
            if(isPlaying==false)
            {
                playTrack();
                isPlaying = true;
                
            }
            else
            {
                waveOutDevice.Play();
            }
            
        }

        private void button6_Click(object sender, EventArgs e)  //pause
        {
            waveOutDevice.Pause();
        }

        private void button2_Click(object sender, EventArgs e)  //stop track
        {
            waveOutDevice.Stop();
            isPlaying = false;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)       //rewind
        {
            WaveStreamExtensions.SetPosition(audioFileReader, (double)trackBar1.Value);
        }

        private void button5_Click(object sender, EventArgs e)  //add track
        {
            System.IO.Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            paths.Add(openFileDialog1.InitialDirectory + openFileDialog1.FileName);
                            fileName = openFileDialog1.FileName.Split('\\');
                            listBox1.Items.Add(fileName[fileName.Count()-1] );
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)    //timer
        {
            if(audioFileReader!=null)
            {
                trackBar1.Value =(audioFileReader.CurrentTime.Hours*3600)+ (audioFileReader.CurrentTime.Minutes*60)+ audioFileReader.CurrentTime.Seconds;
            }
           
        }

        private void button4_Click(object sender, EventArgs e) //next track
        {
            waveOutDevice.Stop();
            audioFileReader.Close();
            if((trackNumber+1)<paths.Count)
            {
                trackNumber++;
            }
            else
            {
                trackNumber = 0;
            }

            playTrack();
        }

        private void button3_Click(object sender, EventArgs e)  //last track
        {
            waveOutDevice.Stop();
            audioFileReader.Close();
            if(trackNumber>0)
            {
                trackNumber--;
            }
            else
            {
                trackNumber = paths.Count-1;
            }

            playTrack();
        }

        private void button7_Click(object sender, EventArgs e)  //delete track
        {
            paths.RemoveAt(listBox1.SelectedIndex);
            listBox1.Items.RemoveAt(listBox1.SelectedIndex);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)  //listbox defines track to play
        {
            trackNumber = listBox1.SelectedIndex;
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (isPlaying == false)
            {
                trackNumber = listBox1.SelectedIndex;
                playTrack();
                isPlaying = true;

            }
            else
            {
                waveOutDevice.Stop();
                audioFileReader.Close();
                trackNumber = listBox1.SelectedIndex;
                playTrack();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)   //random track playing on/off
        {
            if(checkBox1.Checked==true)
            {
                playTrack = playRandom;
            }
            else
            {
                playTrack = play;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            xml.savePlayList();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            xml.LoadPlaylist();
        }

        private void textBox2_Click(object sender, EventArgs e) //clears textbox when click
        {
            textBox2.Clear();
        }

        private void button10_Click(object sender, EventArgs e)
        {

                  progressBar1.Minimum = 0;
                  progressBar1.Maximum = 100;

                  IEnumerable<VideoInfo> videos = DownloadUrlResolver.GetDownloadUrls(textBox2.Text);
                  VideoInfo video;
                       video = videos.First(p => p.VideoType == VideoType.Mp4 && p.Resolution == Convert.ToInt32(comboBox1.Text));
                      SaveFileDialog sv = new SaveFileDialog();
                      sv.AddExtension = true;
                      sv.DefaultExt = "Mp4";
                      if (sv.ShowDialog() == DialogResult.OK)
                      {

                          if (video.RequiresDecryption)
                              DownloadUrlResolver.DecryptDownloadUrl(video);
                          VideoDownloader downloader = new VideoDownloader(video, sv.FileName);
                          downloader.DownloadProgressChanged += DownloadProgresChanged;
                          downloader.DownloadFinished += DownloadFinished;
                          Thread thread = new Thread(() => { downloader.Execute(); }) { IsBackground = true };
                          thread.Start();
                      }
            }

        public void DownloadFinished(object sender, EventArgs e)
        {
            Invoke(new MethodInvoker(delegate ()
            {
                progressBar1.Value = 0;
                progressBar1.Update();
                MessageBox.Show("Download finished!", "Finish");
            }
            ));
        }

        public void DownloadProgresChanged(object sender, ProgressEventArgs e)
        {
            Invoke(new MethodInvoker(delegate ()
           {
               progressBar1.Value = (int)e.ProgressPercentage;
               progressBar1.Update();
           }
            ));
           
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            waveOutDevice.Volume = (float)trackBar2.Value/100;
        }
    }



    public static class WaveStreamExtensions
    {
        
        public static void SetPosition(this WaveStream strm, long position)
        {   
            long adj = position % strm.WaveFormat.BlockAlign;
            long newPos = Math.Max(0, Math.Min(strm.Length, position - adj));
            strm.Position = newPos;
        }

        // Set playback position of WaveStream by seconds
        public static void SetPosition(this WaveStream strm, double seconds)
        {
            strm.SetPosition((long)(seconds * strm.WaveFormat.AverageBytesPerSecond));
        }

        // Set playback position of WaveStream by time (as a TimeSpan)
        public static void SetPosition(this WaveStream strm, TimeSpan time)
        {
            strm.SetPosition(time.TotalSeconds);
        }

        // Set playback position of WaveStream relative to current position
        public static void Seek(this WaveStream strm, double offset)
        {
            strm.SetPosition(strm.Position + (long)(offset * strm.WaveFormat.AverageBytesPerSecond));
        }
    }
}
