using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.InvoiceAndQuotes.ResourceManager;
using Sdl.Community.InvoiceAndQuotes.Templates;

namespace Sdl.Community.InvoiceAndQuotes
{
    public partial class Rates : UserControl
    {
        private List<RateValue> _rates;
        private List<RateValue> _additionalRates;
        private ITemplateRates _currentRatesTemplate;
        private List<TemplateRatesBase> _state;

        public List<TemplateRatesBase> State
        {
            get
            {
                SaveCurrentState();
                return _state;
            }
            set { _state = value; }
        }

        public ITemplateRates CurrentRatesTemplate
        {
            get
            {
                if (_currentRatesTemplate == null)
                    return null;

                UIResources resources = new UIResources(Settings.GetSavedCulture());
                _currentRatesTemplate.ClearRates();
                if (gridRates != null)
                {
                    if (_currentRatesTemplate.Rates == null)
                        _currentRatesTemplate.Rates = new List<RateValue>();
                    foreach (DataGridViewRow row in gridRates.Rows)
                    {
                        _currentRatesTemplate.Rates.Add(new RateValue()
                            {
                                ResourceToken = row.Cells[0].Value == null ? String.Empty : row.Cells[0].Value.ToString(),
                                Type = row.Cells[0].Value == null ? String.Empty : resources.GetString(row.Cells[0].Value.ToString()),
                                Rate = row.Cells[2].Value == null ? 0 : Convert.ToDecimal(row.Cells[2].Value)
                            });
                    }
                }
                if (gridAdditionalRates != null)
                {
                    if (_currentRatesTemplate.AdditionalRates == null)
                        _currentRatesTemplate.AdditionalRates = new List<RateValue>();
                    foreach (DataGridViewRow row in gridAdditionalRates.Rows)
                    {
                        _currentRatesTemplate.AdditionalRates.Add(new RateValue()
                            {
                                ResourceToken = row.Cells[0].Value == null ? String.Empty : row.Cells[0].Value.ToString(),
                                Type = row.Cells[0].Value == null ? String.Empty : resources.GetString(row.Cells[0].Value.ToString()),
                                Rate = row.Cells[2].Value == null ? 0 : Convert.ToDecimal(row.Cells[2].Value)
                            });
                    }
                }
                return _currentRatesTemplate;
            }
        }

        public Rates()
        {
            InitializeComponent();
            this.VisibleChanged += Rates_VisibleChanged;
        }

        void Rates_VisibleChanged(object sender, EventArgs e)
        {
        }

        private void Rates_Load(object sender, EventArgs e)
        {
            gridRates.Visible = gridAdditionalRates.Visible = false;
        }

        public void BindRates(ITemplateRates ratesTemplate, bool saveToState = true)
        {
            if (saveToState) SaveCurrentState();
            _currentRatesTemplate = ratesTemplate;
            _rates = GetRatesFromState(ratesTemplate);
            _additionalRates = GetAdditionalRatesFromState(ratesTemplate);

            BindGrid(gridRates, _rates);
            BindGrid(gridAdditionalRates, _additionalRates);

            if (gridRates.Columns.Count > 0)
                gridRates.Columns[0].Visible = false;
            if (gridAdditionalRates.Columns.Count > 0)
                gridAdditionalRates.Columns[0].Visible = false;
            EnsureUI();
        }

        public void ClearState()
        {
            if (_state != null)
                _state.Clear();
        }

        private List<RateValue> GetRatesFromState(ITemplateRates ratesTemplate)
        {
            if (_state == null)
                return ratesTemplate.GetStandardRates();

            ITemplateRates currentTemplate =
                _state.FirstOrDefault(
                    template => template != null && template.Name == ratesTemplate.Name);
            if (currentTemplate== null)
                return ratesTemplate.GetStandardRates();

            return currentTemplate.Rates;
        }

        private List<RateValue> GetAdditionalRatesFromState(ITemplateRates ratesTemplate)
        {
            if (_state == null)
                return ratesTemplate.GetAdditionalStandardRates();

            ITemplateRates currentTemplate =
                _state.FirstOrDefault(
                    template => template != null && template.Name == ratesTemplate.Name);
            if (currentTemplate == null)
                return ratesTemplate.GetAdditionalStandardRates();

            return currentTemplate.AdditionalRates;
        }

        private void SaveCurrentState()
        {
            if (_currentRatesTemplate == null)
                return;
            if (_state == null)
                _state = new List<TemplateRatesBase>();
            ITemplateRates currentTemplate =
                _state.FirstOrDefault(
                    template => template != null && template.Name == _currentRatesTemplate.Name);
            if (currentTemplate != null)
                _state.Remove((TemplateRatesBase)currentTemplate);

            var resources = new UIResources(Settings.GetSavedCulture());
             _currentRatesTemplate.Rates.Clear();
            foreach (DataGridViewRow row  in gridRates.Rows)
            {
                _currentRatesTemplate.Rates.Add(new RateValue() { ResourceToken = row.Cells[0].Value.ToString(), Type = resources.GetString(row.Cells[0].Value.ToString()), Rate = Convert.ToDecimal(row.Cells[2].Value) });
            }
            _currentRatesTemplate.AdditionalRates.Clear();
            foreach (DataGridViewRow row in gridAdditionalRates.Rows)
            {
                _currentRatesTemplate.AdditionalRates.Add(new RateValue() { ResourceToken = row.Cells[0].Value.ToString(), Type = resources.GetString(row.Cells[0].Value.ToString()), Rate = Convert.ToDecimal(row.Cells[2].Value) });
            }
            _state.Add((TemplateRatesBase)_currentRatesTemplate);
            //add rates
        }

        private void BindGrid(DataGridView dataGridView, List<RateValue> rates)
        {
            dataGridView.DataSource = rates;
            if (rates.Count != 0)
            {
                dataGridView.Columns[0].ReadOnly = true;
                dataGridView.Columns[1].ReadOnly = false;
                dataGridView.Columns[2].ReadOnly = false;
                dataGridView.Height = (dataGridView.RowCount + 1)*dataGridView.Rows[0].Height;
            }
        }

        public void EnsureUI()
        {
            this.Visible = gridRates.Visible = gridAdditionalRates.Visible = true;
            gridRates.Dock = DockStyle.Bottom;

            if (_rates != null && _rates.Count == 0)
            {
                gridRates.Visible = false;
                this.Height = gridAdditionalRates.Height;
            }

            if (_additionalRates != null && _additionalRates.Count == 0)
            {
                gridRates.Dock = DockStyle.Top;
                gridRates.Visible = true;
                gridAdditionalRates.Visible = false;
                this.Height = gridRates.Height;
            }
        }

        public TemplateRatesBase GetSimpleWordTemplateFromState()
        {
            if (_state!= null)
                foreach (var templateRates in _state)
                {
                    if (templateRates is SimpleWordTemplate)
                        return templateRates;
                }

            return  new TemplateRatesBase();
        }

        public void UpdateStateBasedOnUICulture()
        {
            if (_state == null)
                return;
            UIResources resources = new UIResources(Settings.GetSavedCulture());
            foreach (var templateRates in _state)
            {
                foreach (var rateValue in templateRates.Rates)
                {
                    rateValue.Type = resources.GetString(rateValue.ResourceToken);
                }
                foreach (var rateValue in templateRates.AdditionalRates)
                {
                    rateValue.Type = resources.GetString(rateValue.ResourceToken);
                }
            }
        }
    }
}
