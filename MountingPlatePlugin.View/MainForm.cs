using MountingPlatePlugin.Model;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MountingPlatePlugin.View
{
    public class MainForm : Form
    {

            public event Action<MountingPlateParameters> OnBuildRequested;
        private MountingPlateParameters _plateParameters;
        
        // Элементы управления
        private TextBox lengthTextBox;
        private TextBox widthTextBox;
        private TextBox thicknessTextBox;
        private TextBox holesLengthTextBox;
        private TextBox holesWidthTextBox;
        private ComboBox holeTypeComboBox;
        private Button buildButton;
        private Label holeDiameterLabel;
        private Label spacingLengthLabel;
        private Label spacingWidthLabel;
        private Label edgeOffsetLabel;
        private Label totalHolesLabel;

      
        public MainForm()
        {
            InitializeComponent();
            _plateParameters = new MountingPlateParameters();
            InitializeForm();
        }
        
        private void InitializeComponent()
        {
            // Создание элементов
            lengthTextBox = new TextBox();
            widthTextBox = new TextBox();
            thicknessTextBox = new TextBox();
            holesLengthTextBox = new TextBox();
            holesWidthTextBox = new TextBox();
            holeTypeComboBox = new ComboBox();
            buildButton = new Button();
            holeDiameterLabel = new Label();
            spacingLengthLabel = new Label();
            spacingWidthLabel = new Label();
            edgeOffsetLabel = new Label();
            totalHolesLabel = new Label();
            
            // Настройка формы
            this.Text = "Конструктор монтажной пластины";
            this.Size = new Size(500, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            
            // Настройка элементов
            
            // Метки
            var lengthLabel = new Label { Text = "Длина (мм):", Location = new Point(20, 70), Size = new Size(150, 25) };
            var widthLabel = new Label { Text = "Ширина (мм):", Location = new Point(20, 105), Size = new Size(150, 25) };
            var thicknessLabel = new Label { Text = "Толщина (мм):", Location = new Point(20, 140), Size = new Size(150, 25) };
            var holesLengthLabel = new Label { Text = "Отверстий по длине:", Location = new Point(20, 175), Size = new Size(150, 25) };
            var holesWidthLabel = new Label { Text = "Отверстий по ширине:", Location = new Point(20, 210), Size = new Size(150, 25) };
            var holeTypeLabel = new Label { Text = "Тип отверстий:", Location = new Point(20, 245), Size = new Size(150, 25) };
            var calculatedLabel = new Label { Text = "Расчетные значения:", Location = new Point(20, 290), Size = new Size(460, 25), Font = new Font("Arial", 10, FontStyle.Bold) };
            
            // Текстовые поля
            lengthTextBox.Location = new Point(180, 70);
            lengthTextBox.Size = new Size(100, 25);
            lengthTextBox.TextChanged += LengthTextBox_TextChanged;
            
            widthTextBox.Location = new Point(180, 105);
            widthTextBox.Size = new Size(100, 25);
            widthTextBox.TextChanged += WidthTextBox_TextChanged;
            
            thicknessTextBox.Location = new Point(180, 140);
            thicknessTextBox.Size = new Size(100, 25);
            thicknessTextBox.TextChanged += ThicknessTextBox_TextChanged;
            
            holesLengthTextBox.Location = new Point(180, 175);
            holesLengthTextBox.Size = new Size(100, 25);
            holesLengthTextBox.TextChanged += HolesLengthTextBox_TextChanged;
            
            holesWidthTextBox.Location = new Point(180, 210);
            holesWidthTextBox.Size = new Size(100, 25);
            holesWidthTextBox.TextChanged += HolesWidthTextBox_TextChanged;
            
            // ComboBox
            holeTypeComboBox.Location = new Point(180, 245);
            holeTypeComboBox.Size = new Size(150, 25);
            holeTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            holeTypeComboBox.SelectedIndexChanged += HoleTypeComboBox_SelectedIndexChanged;
            
            // Кнопка
            buildButton.Location = new Point(350, 70);
            buildButton.Size = new Size(120, 40);
            buildButton.Text = "Построить";
            buildButton.Click += BuildButton_Click;
            
            // Расчетные значения
            holeDiameterLabel.Location = new Point(40, 320);
            holeDiameterLabel.Size = new Size(440, 20);
            
            spacingLengthLabel.Location = new Point(40, 345);
            spacingLengthLabel.Size = new Size(440, 20);
            
            spacingWidthLabel.Location = new Point(40, 370);
            spacingWidthLabel.Size = new Size(440, 20);
            
            edgeOffsetLabel.Location = new Point(40, 395);
            edgeOffsetLabel.Size = new Size(440, 20);
            
            totalHolesLabel.Location = new Point(40, 420);
            totalHolesLabel.Size = new Size(440, 20);
            
            // Добавление на форму
            this.Controls.AddRange(new Control[] {
                lengthLabel, widthLabel, thicknessLabel, holesLengthLabel, holesWidthLabel, holeTypeLabel, calculatedLabel,
                lengthTextBox, widthTextBox, thicknessTextBox, holesLengthTextBox, holesWidthTextBox,
                holeTypeComboBox, buildButton,
                holeDiameterLabel, spacingLengthLabel, spacingWidthLabel, edgeOffsetLabel, totalHolesLabel
            });

            
        }

       

        
        private void InitializeForm()
        {
            // Начальные значения
            lengthTextBox.Text = _plateParameters.Length.ToString();
            widthTextBox.Text = _plateParameters.Width.ToString();
            thicknessTextBox.Text = _plateParameters.Thickness.ToString();
            holesLengthTextBox.Text = _plateParameters.HolesLength.ToString();
            holesWidthTextBox.Text = _plateParameters.HolesWidth.ToString();
            
            // Настройка ComboBox
            holeTypeComboBox.DataSource = Enum.GetValues(typeof(MountingPlateParameters.HoleType));
            holeTypeComboBox.SelectedItem = MountingPlateParameters.HoleType.Round;
            
            // Блокируем кнопку
            buildButton.Enabled = false;
        }
        
        private void UpdateCalculatedValues()
        {
            try
            {
                holeDiameterLabel.Text = $"Диаметр отверстий: {_plateParameters.HoleDiameter:F2} мм";
                spacingLengthLabel.Text = $"Расстояние по длине: {_plateParameters.HoleSpacingLength:F2} мм";
                spacingWidthLabel.Text = $"Расстояние по ширине: {_plateParameters.HoleSpacingWidth:F2} мм";
                edgeOffsetLabel.Text = $"Отступ от края: {_plateParameters.EdgeOffset:F2} мм";
                totalHolesLabel.Text = $"Всего отверстий: {_plateParameters.TotalHoles}";
                
                buildButton.Enabled = _plateParameters.ValidateAll();
            }
            catch
            {
                buildButton.Enabled = false;
            }
        }
        
    private void BuildButton_Click(object sender, EventArgs e)
{
    try
    {
        // 1. Получаем параметры из TextBox'ов
        _plateParameters.Length = float.Parse(lengthTextBox.Text);
        _plateParameters.Width = float.Parse(widthTextBox.Text);
        _plateParameters.Thickness = float.Parse(thicknessTextBox.Text);
        _plateParameters.HolesLength = int.Parse(holesLengthTextBox.Text);
        _plateParameters.HolesWidth = int.Parse(holesWidthTextBox.Text);
        
        // 2. Показываем сообщение с параметрами
        MessageBox.Show(
            "Пластина построена успешно!\n" +
            $"Длина: {_plateParameters.Length} мм\n" +
            $"Ширина: {_plateParameters.Width} мм\n" +
            $"Толщина: {_plateParameters.Thickness} мм\n" +
            $"Отверстий: {_plateParameters.TotalHoles}\n" +
            $"Тип: {_plateParameters.HoleTypeValue}",
            "Успех",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
        
        
            OnBuildRequested?.Invoke(_plateParameters);
        
        // 4. Закрываем форму
        this.DialogResult = DialogResult.OK;
        this.Close();
    }
    catch (FormatException)
    {
        MessageBox.Show("Ошибка: Неверный формат числа!", "Ошибка", 
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
    catch (ArgumentOutOfRangeException ex)
    {
        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка диапазона",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
        private void ValidateTextBox(TextBox textBox, Action<string> setter, string paramName, bool isInt = false)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(textBox.Text))
                {
                    if (isInt)
                    {
                        int value = int.Parse(textBox.Text);
                        if (paramName.Contains("длин"))
                            _plateParameters.HolesLength = value;
                        else
                            _plateParameters.HolesWidth = value;
                    }
                    else
                    {
                        float value = float.Parse(textBox.Text);
                        setter(value.ToString());
                    }
                    
                    textBox.BackColor = Color.LightGreen;
                }
                else
                {
                    textBox.BackColor = Color.White;
                }
            }
            catch (Exception)
            {
                textBox.BackColor = Color.LightCoral;
            }
            
            UpdateCalculatedValues();
        }
        
        private void LengthTextBox_TextChanged(object sender, EventArgs e)
        {
            ValidateTextBox(lengthTextBox, 
                value => _plateParameters.Length = float.Parse(value),
                "Длина");
        }
        
        private void WidthTextBox_TextChanged(object sender, EventArgs e)
        {
            ValidateTextBox(widthTextBox,
                value => _plateParameters.Width = float.Parse(value),
                "Ширина");
        }
        
        private void ThicknessTextBox_TextChanged(object sender, EventArgs e)
        {
            ValidateTextBox(thicknessTextBox,
                value => _plateParameters.Thickness = float.Parse(value),
                "Толщина");
        }
        
        private void HolesLengthTextBox_TextChanged(object sender, EventArgs e)
        {
            ValidateTextBox(holesLengthTextBox,
                value => _plateParameters.HolesLength = int.Parse(value),
                "Количество отверстий по длине",
                true);
        }
        
        private void HolesWidthTextBox_TextChanged(object sender, EventArgs e)
        {
            ValidateTextBox(holesWidthTextBox,
                value => _plateParameters.HolesWidth = int.Parse(value),
                "Количество отверстий по ширине",
                true);
        }
        
        private void HoleTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (holeTypeComboBox.SelectedItem != null)
            {
                _plateParameters.HoleTypeValue = (MountingPlateParameters.HoleType)holeTypeComboBox.SelectedItem;
                UpdateCalculatedValues();
            }
        }
    }
}
