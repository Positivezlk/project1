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

        Button addButton = CreateButton("Добавить");
        Button editButton = CreateButton("Изменить");
        Button issueButton = CreateButton("Выдать");
        Button returnButton = CreateButton("Вернуть");
        Button damagedButton = CreateButton("Поврежден");
        Button writeOffButton = CreateButton("Списать");
        Button refreshButton = CreateButton("Обновить");
        FlowButtons(315, addButton, editButton, issueButton, returnButton, damagedButton, writeOffButton, refreshButton);

        _grid.SetBounds(15, 60, 1000, 560);
        _grid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        UiTheme.ApplyGridStyle(_grid);

        Controls.AddRange(new Control[] { _searchBox, _statusFilter, addButton, editButton, issueButton, returnButton, damagedButton, writeOffButton, refreshButton, _grid });

        _searchBox.TextChanged += (_, _) => LoadData();
        _statusFilter.SelectedIndexChanged += (_, _) => LoadData();
        refreshButton.Click += (_, _) => LoadData();
        addButton.Click += (_, _) => Save(new TokenEditForm(), false);
        editButton.Click += (_, _) =>
        {
            TokenDevice? token = TokenService.GetById(SelectedId);
            if (token != null)
            {
                Save(new TokenEditForm(token), true);
            }
        };
        issueButton.Click += (_, _) =>
        {
            using TokenIssueForm form = new();
            if (SelectedId > 0 && form.ShowDialog() == DialogResult.OK)
            {
                TokenService.Issue(SelectedId, form.EmployeeId, form.ActNumber, form.Comment, _user);
                LoadData();
            }
        };
        returnButton.Click += (_, _) =>
        {
            if (SelectedId > 0 && MessageHelper.Confirm("Вернуть токен?"))
            {
                TokenService.Return(SelectedId, _user);
                LoadData();
            }
        };
        damagedButton.Click += (_, _) =>
        {
            if (SelectedId > 0 && MessageHelper.Confirm("Отметить токен поврежденным?"))
            {
                TokenService.MarkDamaged(SelectedId, _user);
                LoadData();
            }
        };
        writeOffButton.Click += (_, _) =>
        {
            if (SelectedId > 0 && MessageHelper.Confirm("Списать токен?"))
            {
                TokenService.WriteOff(SelectedId, _user);
                LoadData();
            }
        };

        if (!RoleGuard.CanManageTokens(_user))
        {
            addButton.Enabled = false;
            editButton.Enabled = false;
            issueButton.Enabled = false;
            returnButton.Enabled = false;
            damagedButton.Enabled = false;
            writeOffButton.Enabled = false;
        }
    }

    private static Button CreateButton(string text)
    {
        Button button = new() { Text = text, Top = 13, Width = 95 };
        UiTheme.ApplyButtonStyle(button);
        return button;
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
