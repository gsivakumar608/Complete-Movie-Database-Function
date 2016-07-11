using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {

        private DataClasses1DataContext database = new DataClasses1DataContext();
        private string selectedMenu = "";
        private Dictionary<int, string> movieMap = new Dictionary<int, string>();

        public Form1()
        {
            InitializeComponent();
           // database.Log = Console.Out;
            loadCombox();
            listView1.View = View.List;
            listView2.View = View.Details;
            listView3.View = View.Details;
            listView2.Columns.Add("Id");
            listView2.Columns.Add("Title");
            listView2.Columns.Add("Release year");
            listView2.Columns.Add("Language");
            listView2.Columns.Add("Rating");
            listView3.Columns.Add("Name");
            listView3.Columns.Add("Role");
            MessageBox.Show("Select the criteria from the combox box and enter the value in textbox press submit");

        }

        private void Form1_Load(object sender, EventArgs e)
        {


        }

        private void button1_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            movieMap.Clear();
            string value = textBox1.Text;
            string value1 = textBox2.Text;
            string selectedMenu1 = comboBox2.SelectedItem.ToString();
            List<int> g = getResults(selectedMenu, value);

            Console.WriteLine(g.Count);

            List<int> h = getResults(selectedMenu1, value1);
      
            IEnumerable<int> nm = new List<int>();

            if (h.Count > 0)
                nm = g.Intersect(h);
            else
                nm = g;
           

            IQueryable<MOVy> oDataQuery = database.MOVies;
      
            
            foreach (int a in nm)
            {
                var query = from m in database.MOVies where m.MOVIE_ID == a select m;
                var arr = query.ToArray();
                foreach (var record in arr)
                {
                    movieMap.Add(record.MOVIE_ID,record.TITLE);
                }
            }


            foreach (var s in movieMap)
            {
                ListViewItem n = new ListViewItem(new string[] { s.Value, s.Key.ToString()});
                listView1.Items.Add(n);
            }
     
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            listView2.Items.Clear();
            listView3.Items.Clear();

            selectedMenu = comboBox1.SelectedItem.ToString();
            List<string> menu = new List<string>();
            menu.Add("Director");
            menu.Add("Actor");
            menu.Add("Realese year range");
            menu.Add("Language");
            menu.Add("Genre");
            menu.Add("Title");
            menu.Add("Rating");

            if (selectedMenu.Equals("Director"))
                menu.Remove("Director");
            else if (selectedMenu.Equals("Actor"))
                menu.Remove("Actor");
            else if(selectedMenu.Equals("Realese year range"))
                menu.Remove("Actor");
            else if(selectedMenu.Equals("Language"))
                menu.Remove("Language");
            else if(selectedMenu.Equals("Genre"))
                menu.Remove("Genre");
            else if(selectedMenu.Equals("Title"))
                menu.Remove("Title");
            else if(selectedMenu.Equals("Rating"))
                menu.Remove("Rating");

            comboBox2.DataSource = menu;
            comboBox2.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboBox2.AutoCompleteSource = AutoCompleteSource.ListItems;

        }

        private void loadCombox()
        {
            List<string> menu = new List<string>();

            menu.Add("Director");
            menu.Add("Actor");
            menu.Add("Realese year range");
            menu.Add("Language");
            menu.Add("Genre");
            menu.Add("Title");
            menu.Add("Rating");

            comboBox1.DataSource = menu;
            comboBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboBox1.AutoCompleteSource = AutoCompleteSource.ListItems;

        }



        private void textBox1_KeyDown(object sender, EventArgs e)
        {

            if (selectedMenu.StartsWith("Rea"))
            {
                if (textBox1.TextLength == 4)
                {
                    string val = textBox1.Text;
                    textBox1.Text = val + ",";
                    textBox1.Select(textBox1.TextLength, 0);
                }

            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            listView2.Items.Clear();
            listView3.Items.Clear();
            listBox1.Items.Clear();
        }

        private List<int> getResults(String selectedMenu, String value)
        {
            List<int> result = new List<int>();

            if (selectedMenu.Equals("Actor") || selectedMenu.Equals("Director"))
            {
                var query = (from MovieMaker in database.MOVIEMAKERs where MovieMaker.MOVIE_ROLE == selectedMenu && MovieMaker.MAKER_ID == (from Maker in database.MAKERs where Maker.MAKER_NAME == value select Maker.MAKER_ID).FirstOrDefault() select MovieMaker.MOVIE_ID);
                result = query.ToList();
            }
            else if (selectedMenu.Equals("Genre"))
            {
                var query = (from genre in database.GENREs where genre.GENRE1 == value select genre.MOVIE_ID);
                result = query.ToList();
            }

            else if (selectedMenu.Equals("Rating"))
            {
                var query = (from movies in database.MOVies where movies.RATING == int.Parse(value) select movies.MOVIE_ID);
                result = query.ToList();
            }
            else if (selectedMenu.Equals("Language"))
            {
                var query = (from movies in database.MOVies where movies.SPOKEN_LANGUAGE == value select movies.MOVIE_ID);
                result = query.ToList();
            }
            else if (selectedMenu.Equals("Title"))
            {
                var query = (from movies in database.MOVies where movies.TITLE.Contains(value) select movies.MOVIE_ID);
                result = query.ToList();
            }
            else if (selectedMenu.StartsWith("Rea"))
            {
                string[] range = value.Split(',');
                var query = (from movies in database.MOVies where movies.RELEASE_YEAR >= int.Parse(range[0]) && movies.RELEASE_YEAR <= int.Parse(range[1]) select movies.MOVIE_ID);
                result = query.ToList();
            }
            return result;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
   
            if (listView1.SelectedItems.Count > 0)
            {
                listView2.Items.Clear();
                listView3.Items.Clear();
                List<string> genreList = new List<string>();
                var selectedItem = listView1.SelectedItems[0];
                int id = int.Parse(selectedItem.SubItems[1].Text);
                var query = from genre in database.GENREs where genre.MOVIE_ID == id select genre;
                var queryArr = query.ToArray();
               
                foreach (var quer in queryArr)
                {
                    genreList.Add(quer.GENRE1);
                }
                listBox1.DataSource = genreList;

                
                var que = from m in database.MOVies where m.MOVIE_ID == id select m;
                    var arr = que.ToArray();
                    foreach (var a in arr)
                    {
                        ListViewItem movieDetail = new ListViewItem(new string[]{a.MOVIE_ID.ToString(),a.TITLE,a.RELEASE_YEAR.ToString(),a.SPOKEN_LANGUAGE,a.RATING.ToString()});
                        listView2.Items.Add(movieDetail);
                    }


                    var que1 = from m in database.MOVIEMAKERs where m.MOVIE_ID == id select m;
                    var arr1 = que1.ToArray();
                    foreach (var a in arr1)
                    {
                        var que2 = from m in database.MAKERs where m.MAKER_ID == a.MAKER_ID select m.MAKER_NAME;
                        foreach(var v in que2) {
                             ListViewItem makerDetail = new ListViewItem(new string[] { v, a.MOVIE_ROLE });
                             listView3.Items.Add(makerDetail);
                        }
                       
                    }
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            listView2.Items.Clear();
            listView3.Items.Clear();
            listBox1.Items.Clear();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            listView2.Items.Clear();
            listView3.Items.Clear();
            listBox1.Items.Clear();
        }
    }
}
