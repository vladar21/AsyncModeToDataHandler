using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace AsyncModeToDataHandler
{
    public partial class Form1 : Form
    {
        // список для хранения данных из базы
        List<string[]> data;

        public Form1()
        {
            InitializeComponent();

            label2.Text = "Демонстрация асинхронности - надпись здесь (в лейбл2) появится раньше, чем будет выполненна загрузка данных";

            button1.Click += (s, e) => Task.Run(() =>
            {
                // демонстрация асинхронности - надпись в лейбл2 появится раньше, чем будет выполненна загрузка данных 
                label2.Text = "Вывод этой надписи в коде происходит сразу после нажатия кнопки запускающей загрузку данных...";
            });

        }
        // асинхронная функция для загрузки данных из базы в список data
        private async Task LoadData()
        {
            // задержка времени выполнения
            await Task.Delay(2000);

            await Task.Run(() =>
            {
                DataClasses1DataContext db = new DataClasses1DataContext();
                var queryResults = from c in db.srcbooks
                                   select new
                                   {
                                       ID = c.id,
                                       Name = c.name,
                                       Price = c.price,
                                       Izdatel = c.izd
                                   };

                data = new List<string[]>();

                foreach (var item in queryResults)
                {
                    data.Add(new string[4]);
                    data[data.Count - 1][0] = item.ID.ToString();
                    data[data.Count - 1][1] = item.Name.ToString();
                    data[data.Count - 1][2] = item.Price.ToString();
                    data[data.Count - 1][3] = item.Izdatel.ToString();
                }

            });            
        }
        // при нажатии кнопки происходит асинхронная загрузка данных из базы, заполнение датагрида и комбобокса
        private async void button1_Click(object sender, EventArgs e)
        {
            // загрузка данных из базы
            await LoadData();

            // заполнение списка
            for (int i = 0; i < data[0].Count(); i++)
            {
                switch (i)
                {
                    case 0:
                        dataGridView1.Columns.Add("id", null);
                        break;
                    case 1:
                        dataGridView1.Columns.Add("name", null);
                        break;
                    case 2:
                        dataGridView1.Columns.Add("price", null);
                        break;
                    case 3:
                        dataGridView1.Columns.Add("izdatel", null);
                        break;
                    default:
                        dataGridView1.Columns.Add("id", null);
                        break;
                }
            }
            // выдача данных в датагрид
            foreach (string[] s in data)
                dataGridView1.Rows.Add(s);

            // заполняем комбобокс 
            var qR = data.GroupBy(izd => izd[3])
                        .Select(g => new { izdatel = g.Key, qtybooks = g.Count() });
            // альтернативный способ выборки данных для комбобокса
            //var qR = from izd in data
            //                   group izd by izd[3] into g
            //                   select new { izdatel = g.Key, qtybooks = g.Count() };    
            foreach (var item in qR)
            {
                comboBox1.Items.Add(item);
            }

        }

        // обработка событи выборки издателя из комбобокса, вывод в лейбл1 выбранного значения
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            string name = comboBox.SelectedItem.ToString();
            label1.Text = name;
        }
    }

}
