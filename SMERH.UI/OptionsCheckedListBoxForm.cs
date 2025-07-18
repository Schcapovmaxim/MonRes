using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileSelection
{
    public partial class OptionsCheckedListBoxForm : Form
    {
        public event Action<List<string>> CheckedItemsChanged; // событие "на какой-то чекбокс нажали"

        public OptionsCheckedListBoxForm()
        {
            InitializeComponent();
        }

        public void SetCheckedItems(List<string> itemsToCheck) // получение с основной формы информации о уже нажатых чекбоксах
        {
            for (int i = 0; i < checkedListBoxfunction.Items.Count; i++) // проход по списку чекбоксов
            {
                var item = checkedListBoxfunction.Items[i].ToString(); // получение текущего чекбокса
                checkedListBoxfunction.SetItemChecked(i, itemsToCheck.Contains(item)); // назначение статуса нажатости чекбокса
            }
        }

        private void checkedListBoxfunction_MouseUp(object sender, MouseEventArgs e) // обработка нажатия на чекбокс
        {
            CheckedListBox clb = sender as CheckedListBox; // получение доступа к чекбоксам
            int index = clb.IndexFromPoint(e.Location); // получение индекса нажатого чекбокса

            if (index != ListBox.NoMatches) // если попали не в пустое место списка чекбоксов
            {
                bool currentCheckState = clb.GetItemChecked(index); // получение текущего значения чекбокса
                clb.SetItemChecked(index, !currentCheckState); // переключение значения этого чекбокса

                BeginInvoke(new Action(() => // тут список выбранных чекбоксов идёт на основную форму
                {
                    var checkedItems = checkedListBoxfunction.CheckedItems // получение выбранных чекбоксов
                        .Cast<object>()
                        .Select(item => item.ToString())
                        .ToList();

                    CheckedItemsChanged?.Invoke(checkedItems); // отправка выбранных чекбоксов на основную форму
                }));
            }
        }
    }
}
