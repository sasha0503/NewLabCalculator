using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.Json;

namespace LabCalculator
{
    public partial class Form1 : Form
    {
        private Cell currentCell;
        private int currentRow;
        private int currentColumn;
        public static string[] alphabet;
        private Dictionary<string, int> colNameToColIndex;
        public int CurrentRow
        {
            get { return currentRow; }
            set { currentRow = value; }
        }
        public int CurrentColumn
        {
            get { return currentColumn; }
            set { currentColumn = value; }
        }
        public Form1(int cols, int rows)
        {
            colNameToColIndex = new Dictionary<string, int>();

            alphabet = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", 
                                      "J", "K", "L", "M", "N", "O", "P", "Q", "R",
                                      "S", "T", "U", "V", "W", "X", "Y", "Z" };
            
            InitializeComponent();

            int columnCount = cols;

            DataGridViewColumn col;
            
            for (int i = 0; i < columnCount; i++)
            {
                col = new DataGridViewColumn(new Cell());
                dataGridView1.Columns.Insert(0, col);
            }

            dataGridView1.RowCount = rows;
            
            FillColumnNames(); // start from A
            FillRowNames(); // start from 0
        }

        // DataGridView event handlers
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
        }
        private void dataGridView1_InsertRow(int index)
        {
            if (index == dataGridView1.RowCount)
            {
                dataGridView1.RowCount++;
                dataGridView1.Rows[dataGridView1.RowCount - 1].HeaderCell.Value = (dataGridView1.RowCount - 1).ToString();
            }
            else
            {
                dataGridView1.Rows.Insert(index);
            }
        }

        private void dataGridView1_InsertCol(int index)
        {
            DataGridViewColumn col = new DataGridViewColumn(new Cell());
            dataGridView1.Columns.Insert(index, col);
        }

        private void dataGridView1_DelRow(int index)
        {
            dataGridView1.Rows.RemoveAt(index);
        }

        private void dataGridView1_DelCol(int index)
        {
            dataGridView1.Columns.RemoveAt(index);
        }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex == -1 || e.ColumnIndex == -1)
            {
                if (e.Button == MouseButtons.Right)
                {
                    // Get mouse position relative to the vehicles grid
                    var relativeMousePosition = dataGridView1.PointToClient(Cursor.Position);

                    currentRow = e.RowIndex;
                    currentColumn = e.ColumnIndex;
                    // Show the context menu
                    contextMenuStrip1.Show(dataGridView1, relativeMousePosition);
                }
            }
            else
            {
                currentCell = (Cell)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                label1.Text = GetColumnName(e.ColumnIndex) + Convert.ToString(e.RowIndex);
            }
        }

        // Form event handlers
        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Left;
        }

        // add new column / row
        private void AddMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentRow == -1 && currentColumn != -1)
                {
                    dataGridView1_InsertCol(currentColumn + 1);
                    FillColumnNames();
                    dataGridView1.Columns[currentColumn + 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    dataGridView1.Columns[currentColumn + 1].CellTemplate = new Cell();
                }
                if (currentColumn == -1 && currentRow != -1)
                {
                    dataGridView1_InsertRow(currentRow + 1);
                    FillRowNames();
                }
            }
            catch (IndexOutOfRangeException exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        // delete  column / row
        private void DelMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentRow == -1 && currentColumn != -1)
                {
                    dataGridView1_DelCol(currentColumn);
                    FillColumnNames();
                    
                }
                else if (currentColumn == -1 && currentRow != -1)
                {
                    dataGridView1_DelRow(currentRow);
                    FillRowNames();
                    
                }
            }
            catch (IndexOutOfRangeException exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        //event handlers
        private void button1_Click(object sender, EventArgs e)
        {
            if (ExpressionTextBox.Text != "")
            {
                foreach (Cell cell in dataGridView1.SelectedCells)
                {
                    cell.Expression = ExpressionTextBox.Text;
                    cell.Value = cell.Expression;
                }
                ExpressionTextBox.Text = "";
            }
            else
            {
                currentCell.Expression = Convert.ToString(currentCell.Value);
            }
        }

        public void CellValueChanged(object sender, DataGridViewCellMouseEventArgs e)
        {
        }
        public void DataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            
        }

        // Top Menu event handlers.
        private void SaveMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();


                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    List<string> infoAboutTable = new List<string>();

                    infoAboutTable.Add(dataGridView1.RowCount.ToString());
                    infoAboutTable.Add(dataGridView1.ColumnCount.ToString());

                    System.Xml.Serialization.XmlSerializer writer =
                        new System.Xml.Serialization.XmlSerializer(typeof(List<string>));

                    var path = sfd.FileName;
                    FileStream file = File.Create(path);

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        foreach (Cell cell in row.Cells)
                        {
                            infoAboutTable.Add(cell.Expression);
                        }
                    }
                    writer.Serialize(file, infoAboutTable);
                    file.Close();
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string fileName = ofd.FileName;
                using (Stream reader = new FileStream(fileName, FileMode.Open))
                {

                    System.Xml.Serialization.XmlSerializer deserializer =
                        new System.Xml.Serialization.XmlSerializer(typeof(List<string>));
                    // Call the Deserialize method to restore the object's state.
                    List<string> infoAboutTable = (List<string>)deserializer.Deserialize(reader);
                    dataGridView1.RowCount = int.Parse(infoAboutTable[0]);
                    dataGridView1.ColumnCount = int.Parse(infoAboutTable[1]);

                    int i = 2; // index of expression
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        foreach (Cell cell in row.Cells)
                        {
                            cell.Expression = infoAboutTable[i++];
                        }
                    }
                }
                
            }
        }



        // Get DataGridView to access its data (designer made it private, no other way to work around)
        public DataGridView getDataGridView()
        {
            return dataGridView1;
        }

        public Dictionary<string, int> getColNameToColIndex()
        {
            return colNameToColIndex;
        }

        private string GetColumnName(int i)
        {
            Stack<int> base27Number;
            base27Number = ConvertToBase27(i + 1);
            StringBuilder name = new StringBuilder("");
            while (base27Number.Count != 0)
            {
                int nextPartOfNumber = base27Number.Pop();
                string character = alphabet[nextPartOfNumber - 1];
                name.Append(character);
            }
            return name.ToString();
        }

        private void FillColumnNames()
        {
            DataGridViewColumn col;
            colNameToColIndex.Clear();
            int numberToConvert = 1;
            Stack<int> base27Number;
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                base27Number = ConvertToBase27(numberToConvert);
                col = dataGridView1.Columns[i];
                while (base27Number == null)
                {
                    numberToConvert++;
                    base27Number = ConvertToBase27(numberToConvert);
                }
                StringBuilder name = new StringBuilder("");
                while (base27Number.Count != 0)
                {
                    int nextPartOfNumber = base27Number.Pop();
                    string character = alphabet[nextPartOfNumber - 1];
                    name.Append(character);
                }
                col.Name = name.ToString();
                col.HeaderText = name.ToString();
                colNameToColIndex.Add(name.ToString(), i);
                numberToConvert++;
            }
        }

        // fill the starting table
        private void FillRowNames()
        {
            DataGridViewRow row;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                row = dataGridView1.Rows[i];
                row.HeaderCell.Value = i.ToString();
            }
        }

        // Convert number to base 27.
        private Stack<int> ConvertToBase27(int number)
        {
            const int BASE = 27;
            Stack<int> base27Representation = new Stack<int>();
            while (number / BASE > 0)
            {
                if (number % BASE == 0) return null;
                base27Representation.Push(number % BASE);
                number /= BASE;
            }
            if (number % BASE == 0) return null;
            base27Representation.Push(number % BASE);
            return base27Representation;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void ExpressionTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ExpressionTextBox.Text != "")
            {
                foreach (Cell cell in dataGridView1.SelectedCells)
                {
                    cell.Value = ExpressionTextBox.Text;
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void Menu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    foreach (Cell cell in row.Cells)
                    {
                        currentRow = cell.RowIndex;
                        currentColumn = cell.ColumnIndex;
                        cell.Selected = false;
                        cell.Value = Calculator.Evaluate(cell.Expression);
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (Cell cell in row.Cells)
                {
                    currentRow = cell.RowIndex;
                    currentColumn = cell.ColumnIndex;
                    cell.Selected = false;
                    cell.Value = cell.Expression;
                }
            }
        }
    }
}
