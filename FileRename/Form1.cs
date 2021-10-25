using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileRename
{
    public partial class btnXmit : Form
    {
        public btnXmit()
        {
            InitializeComponent();
        }
        DataTable table = new DataTable();
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string[] sourceDirectoryFiles = Directory.GetFiles(txtSource.Text);
                string[] sourceDirectorysubfolders = Directory.GetDirectories(txtdestination.Text);
                string sourcedirectory = txtSource.Text;
                string targetdirectory = txtdestination.Text;
                Copy(sourcedirectory, targetdirectory);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        string file = "";
        public void Copy(string sourcePath, string targetPath)
        {

            try
            {
                foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                    Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));

                string newPath;
                foreach (string srcPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
                {
                    newPath = srcPath.Replace(sourcePath, targetPath);
                    //newPath = newPath.Insert(newPath.LastIndexOf("\\") + 1); //prefixing 'a'
                    file = "";
                    string path = newPath;
                    file = Path.GetFileNameWithoutExtension(path);
                    //int pos = sourcePath.LastIndexOf("/") + 1;
                    string directoryName = Path.GetFileNameWithoutExtension(sourcePath);
                    string whercondtion = "docs/" + directoryName + "/" + file + ".pdf";
                    string whercondtion1 = "docs/" + directoryName + "/" + file + ".doc";
                    string whercondtion2 = "docs/" + directoryName + "/" + file + ".docx";
                    string whercondtion3 = "docs/" + directoryName + "/" + file + ".xls";
                    string name = (from r in table.AsEnumerable()
                                   where (r.Field<string>("TreeChildNodeURL") == whercondtion
                                   || r.Field<string>("TreeChildNodeURL") == whercondtion1 ||
                                   r.Field<string>("TreeChildNodeURL") == whercondtion2
                                   || r.Field<string>("TreeChildNodeURL") == whercondtion3)
                                   //&& r.Field<string>("Application") == directoryName
                                   select r.Field<string>("TreeChildNodeName")).FirstOrDefault();
                    //DataRow[] dr = table.Select("TreeChildNodeURL=docs" + "/" + directoryName + "/ " + file+".pdf");
                    string TreeNodeName = table.AsEnumerable().Where(r => r.Field<string>("TreeChildNodeURL") == whercondtion
                    || r.Field<string>("TreeChildNodeURL") == whercondtion1 ||
                                   r.Field<string>("TreeChildNodeURL") == whercondtion2
                                   || r.Field<string>("TreeChildNodeURL") == whercondtion3).
                                   Select(r => r.Field<string>("TreeNodeName")).FirstOrDefault();
                    if (name == null)
                        continue;

                    string newname = TreeNodeName + "\\" + name;
                    string pathnew = Path.GetDirectoryName(path) + "\\" + TreeNodeName + "\\";
                    string[] paths = { path, TreeNodeName, name };
                    string fullPath = Path.Combine(paths);
                    string NewPath = path.Replace(file, newname);

                    //string NewPath = path.Replace(file, name);
                    if (File.Exists(NewPath))
                        File.Delete(NewPath);

                    if (!Directory.Exists(pathnew))
                        Directory.CreateDirectory(pathnew);
                    try
                    {

                        File.Copy(srcPath, NewPath, true);
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
                MessageBox.Show("Completed");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString() + file);
            }
        }

        DataTable dt = new DataTable();
        private void btnXmit_Load(object sender, EventArgs e)
        {
            SqlConnection cnn = new SqlConnection();
            try
            {
                string connetionString;
                connetionString = @"Data Source=mhccmsdb0006;Initial Catalog=mhcc_policy;User ID=sqladmin;Password=phnsWeb08";
                cnn = new SqlConnection(connetionString);
                cnn.Open();
                using (var command = new SqlCommand("SELECT * FROM [ConversionFileNameFrom201001 20210501]", cnn))
                {
                    // Loads the query results into the table
                    dt.Load(command.ExecuteReader());
                }
                //txtdestination.Text = @"\\MHC-MSASCTLR02.mclaren.org\HCL_UPMProfiles$\Redirection\fqms38\Documents\Test";
                //txtdestination.Text = @\\MHC-MSASCTLR02.mclaren.org\HCL_UPMProfiles$\Redirection\fqms38\DocumentPDF";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                cnn.Close();
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
