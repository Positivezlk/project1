using System;
using System.Drawing;
using System.Windows.Forms;
using CertDesk.Common;
using CertDesk.Forms;
using CertDesk.Models;
using CertDesk.Services;

namespace CertDesk.Views;

public partial class TokensView : UserControl
{
    private readonly CurrentUser _user;
    private readonly DataGridView _grid = new();
    private readonly TextBox _searchBox = new();
    private readonly ComboBox _statusFilter = new();
    private readonly Button _addButton = new();
    private readonly Button _editButton = new();
    private readonly Button _issueButton = new();
    private readonly Button _returnButton = new();
    private readonly Button _damagedButton = new();
    private readonly Button _writeOffButton = new();
    private readonly Button _refreshButton = new();

    public TokensView(CurrentUser user)
    {
        _user = user;
        InitializeComponent();
        Build();
        LoadData();
    }

    private int SelectedId => _grid.CurrentRow == null ? 0 : Convert.ToInt32(_grid.CurrentRow.Cells["ID"].Value);

    private void Build()
    {
        BackColor = UiTheme.Light;

        _searchBox.SetBounds(15, 15, 160, 25);
        _statusFilter.SetBounds(185, 15, 120, 25);
        _statusFilter.Items.AddRange(new object[] { "Все", "storage", "issued", "damaged", "written_off" });
        _statusFilter.SelectedIndex = 0;

        ConfigureButton(_addButton, "Добавить", 95);
        ConfigureButton(_editButton, "Изменить", 95);
        ConfigureButton(_issueButton, "Выдать", 95);
        ConfigureButton(_returnButton, "Вернуть", 95);
        ConfigureButton(_damagedButton, "Поврежден", 95);
        ConfigureButton(_writeOffButton, "Списать", 95);
        ConfigureButton(_refreshButton, "Обновить", 95);
        FlowButtons(315, _addButton, _editButton, _issueButton, _returnButton, _damagedButton, _writeOffButton, _refreshButton);

        _grid.SetBounds(15, 60, 1000, 560);
        _grid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        UiTheme.ApplyGridStyle(_grid);

        Controls.AddRange(new Control[] { _searchBox, _statusFilter, _addButton, _editButton, _issueButton, _returnButton, _damagedButton, _writeOffButton, _refreshButton, _grid });

        _searchBox.TextChanged += (_, _) => LoadData();
        _statusFilter.SelectedIndexChanged += (_, _) => LoadData();
        _refreshButton.Click += (_, _) => LoadData();
        _addButton.Click += (_, _) => Save(new TokenEditForm(), false);
        _editButton.Click += (_, _) =>
        {
            TokenDevice? token = TokenService.GetById(SelectedId);
            if (token != null)
            {
                Save(new TokenEditForm(token), true);
            }
        };
        _issueButton.Click += (_, _) =>
        {
            using TokenIssueForm form = new();
            if (SelectedId > 0 && form.ShowDialog() == DialogResult.OK)
            {
                TokenService.Issue(SelectedId, form.EmployeeId, form.ActNumber, form.Comment, _user);
                LoadData();
            }
        };
        _returnButton.Click += (_, _) =>
        {
            if (SelectedId > 0 && MessageHelper.Confirm("Вернуть токен?"))
            {
                TokenService.Return(SelectedId, _user);
                LoadData();
            }
        };
        _damagedButton.Click += (_, _) =>
        {
            if (SelectedId > 0 && MessageHelper.Confirm("Отметить токен поврежденным?"))
            {
                TokenService.MarkDamaged(SelectedId, _user);
                LoadData();
            }
        };
        _writeOffButton.Click += (_, _) =>
        {
            if (SelectedId > 0 && MessageHelper.Confirm("Списать токен?"))
            {
                TokenService.WriteOff(SelectedId, _user);
                LoadData();
            }
        };

        if (!RoleGuard.CanManageTokens(_user))
        {
            _addButton.Enabled = false;
            _editButton.Enabled = false;
            _issueButton.Enabled = false;
            _returnButton.Enabled = false;
            _damagedButton.Enabled = false;
            _writeOffButton.Enabled = false;
        }
    }

    private static void ConfigureButton(Button button, string text, int width)
    {
        button.Text = text;
        button.Top = 13;
        button.Width = width;
        UiTheme.ApplyButtonStyle(button);
    }

    private static void FlowButtons(int x, params Button[] buttons)
    {
        foreach (Button button in buttons)
        {
            button.Left = x;
            x += 101;
        }
    }

    private void Save(TokenEditForm form, bool edit)
    {
        try
        {
            if (form.ShowDialog() == DialogResult.OK)
            {
                if (edit)
                {
                    TokenService.Update(form.Token, _user);
                }
                else
                {
                    TokenService.Create(form.Token, _user);
                }

                LoadData();
            }
        }
        catch (Exception ex)
        {
            MessageHelper.Error("Не удалось сохранить. Проверьте уникальность номера. " + ex.Message);
        }
    }

    private void LoadData() => _grid.DataSource = TokenService.Search(_searchBox.Text, _statusFilter.Text);
}
