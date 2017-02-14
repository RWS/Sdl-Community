using System;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.Qualitivity.Tracking;
using Sdl.Community.Structures.Profile;
using Sdl.Community.Structures.PropertyView;
using Activity = Sdl.Community.Structures.Projects.Activities.Activity;
using Project = Sdl.Community.Structures.Projects.Project;

namespace Sdl.Community.Qualitivity.Panels.Properties
{
    public partial class QualitivityViewPropertiesControl : UserControl
    {

        public QualitivityViewPropertiesControl()
        {
            InitializeComponent();

        }

        public void UpdateActivityPropertiesViewer()
        {
            try
            {
                if (QualitivityViewPropertiesController.NavigationTreeView.SelectedNode != null)
                {
                    var apv = new ActivityPropertiesView();
                    if (QualitivityViewPropertiesController.ObjectListView.SelectedObjects.Count > 0)
                    {
                        #region  |  ActivityPropertiesView  |


                        var tpa = (Activity)QualitivityViewPropertiesController.ObjectListView.SelectedObjects[0];
                        var tp = Helper.GetProjectFromId(tpa.ProjectId);

                        if (tp.CompanyProfileId > -1)
                        {
                            #region  |  client  |

                            var cpi = Helper.GetClientFromId(tp.CompanyProfileId);
                            var _pemRate = Tracked.Settings.LanguageRateGroups.FirstOrDefault(pemr => pemr.Id == cpi.ProfileRate.LanguageRateId);
                            var client = new Client
                            {
                                ClientId = cpi.Id.ToString(),
                                ClientName = cpi.Name
                            };


                            var clientAddress = new Address
                            {
                                AddressStreet = cpi.Street,
                                AddressZip = cpi.Zip,
                                AddressCity = cpi.City,
                                AddressState = cpi.State,
                                AddressCountry = cpi.Country
                            };
                            client.ClientAddress = clientAddress;


                            var contactDetails = new ContactDetails
                            {
                                EMail = cpi.Email,
                                WebPage = cpi.Web,
                                Phone = cpi.Phone,
                                FAX = cpi.Fax
                            };
                            client.ContactDetails = contactDetails;

                            client.ClientVat = cpi.VatCode;
                            client.ClientTax = cpi.TaxCode;

                            var defaultRates = new DefaultRates
                            {
                                HrRate = cpi.ProfileRate.HourlyRateRate,
                                HrRateCurrency = cpi.ProfileRate.HourlyRateCurrency,
                                PemRate = _pemRate != null ? _pemRate.Name : string.Empty
                            };

                            client.DefaultRates = defaultRates;

                            apv.ClientProfile = client;
                            #endregion
                        }

                        #region  |  project  |

                        var project = new Sdl.Community.Structures.PropertyView.Project
                        {
                            ProjectId = tp.Id.ToString(),
                            ProjectIdStudio = tp.StudioProjectId,
                            ProjectName = tp.Name,
                            ProjectDescription = tp.Description,
                            ProjectStatus = tp.ProjectStatus,
                            ProjectDateCreated = tp.Created,
                            ProjectDateDue = tp.Due,
                            ProjectDateComplated = tp.Completed,
                            ProjectActivitesCount = tp.Activities.Count.ToString()
                        };





                        apv.ProjectDetails = project;
                        #endregion

                        #region  |  activity  |

                        var activityDetails = new Sdl.Community.Structures.PropertyView.Activity
                        {
                            ActivityDocuments = tpa.Activities.Count,
                            ActivityId = tpa.Id.ToString(),
                            ActivityName = tpa.Name,
                            ActivityDescription = tpa.Description
                        };




                        var hourlyRate = new HourlyRate
                        {
                            HrCkd = tpa.HourlyRateChecked,
                            hr_currency = tpa.DocumentActivityRates.HourlyRateCurrency,
                            hr_quantity = Convert.ToDecimal(Math.Round(tpa.DocumentActivityRates.HourlyRateQuantity, 3)),
                            hr_rate = Convert.ToDecimal(tpa.DocumentActivityRates.HourlyRateRate),
                            hr_total = Convert.ToDecimal(tpa.DocumentActivityRates.HourlyRateTotal)
                        };
                        activityDetails.HourlyRate = hourlyRate;


                        var pemRate = new LanguageRateGroup
                        {
                            PemCkd = tpa.LanguageRateChecked,
                            PemCurrency =
                            tpa.DocumentActivityRates.LanguageRateId > -1
                                ? tpa.DocumentActivityRates.LanguageRateCurrency
                                : string.Empty,
                            PemName =
                            tpa.DocumentActivityRates.LanguageRateId > -1
                                ? tpa.DocumentActivityRates.LanguageRateName
                                : string.Empty,
                            PemTotal =
                                Convert.ToDecimal(tpa.DocumentActivityRates.LanguageRateId > -1
                                    ? tpa.DocumentActivityRates.LanguageRateTotal
                                    : 0)
                        };
                        activityDetails.PemRate = pemRate;




                        activityDetails.ActivityDateStart = tpa.Started;
                        activityDetails.ActivityDateEnd = tpa.Stopped;

                        activityDetails.ActivityStatus = tpa.ActivityStatus.ToString();
                        activityDetails.ActivityBillable = tpa.Billable;

                        activityDetails.ActivityTotal = Math.Round((tpa.HourlyRateChecked ? tpa.DocumentActivityRates.HourlyRateTotal : 0)
                            + (tpa.LanguageRateChecked ? tpa.DocumentActivityRates.LanguageRateTotal : 0), 2) + tpa.DocumentActivityRates.HourlyRateCurrency;

                        apv.ActivityDetails = activityDetails;
                        #endregion

                        propertyGrid1.SelectedObject = apv;

                        #endregion
                    }
                    else
                    {
                        if (QualitivityViewPropertiesController.NavigationTreeView.SelectedNode.Tag != null
                            && QualitivityViewPropertiesController.NavigationTreeView.SelectedNode.Tag.GetType() == typeof(Project))
                        {

                            var tp = (Project)QualitivityViewPropertiesController.NavigationTreeView.SelectedNode.Tag;
                            if (tp.CompanyProfileId > -1)
                            {
                                #region  |  client  |

                                var cpi = Helper.GetClientFromId(tp.CompanyProfileId);
                                var pemRate = Tracked.Settings.LanguageRateGroups.FirstOrDefault(pemr => pemr.Id == cpi.ProfileRate.LanguageRateId);
                                var client = new Client {ClientId = cpi.Id.ToString()};



                                var clientAddress = new Address
                                {
                                    AddressStreet = cpi.Street,
                                    AddressZip = cpi.Zip,
                                    AddressCity = cpi.City,
                                    AddressState = cpi.State,
                                    AddressCountry = cpi.Country
                                };
                                client.ClientAddress = clientAddress;


                                var contactDetails = new ContactDetails
                                {
                                    EMail = cpi.Email,
                                    WebPage = cpi.Web,
                                    Phone = cpi.Phone,
                                    FAX = cpi.Fax
                                };
                                client.ContactDetails = contactDetails;

                                client.ClientVat = cpi.VatCode;
                                client.ClientTax = cpi.TaxCode;

                                var defaultRates = new DefaultRates
                                {
                                    HrRate = cpi.ProfileRate.HourlyRateRate,
                                    HrRateCurrency = cpi.ProfileRate.HourlyRateCurrency,
                                    PemRate = pemRate != null ? pemRate.Name : string.Empty
                                };

                                client.DefaultRates = defaultRates;

                                apv.ClientProfile = client;
                                #endregion
                            }
                            #region  |  project  |

                            var project = new Sdl.Community.Structures.PropertyView.Project
                            {
                                ProjectId = tp.Id.ToString(),
                                ProjectIdStudio = tp.StudioProjectId,
                                ProjectName = tp.Name,
                                ProjectDescription = tp.Description,
                                ProjectStatus = tp.ProjectStatus,
                                ProjectDateCreated = tp.Created,
                                ProjectDateDue = tp.Due,
                                ProjectDateComplated = tp.Completed,
                                ProjectActivitesCount = tp.Activities.Count.ToString()
                            };





                            apv.ProjectDetails = project;
                            #endregion
                            propertyGrid1.SelectedObject = apv;

                        }
                        else if (QualitivityViewPropertiesController.NavigationTreeView.SelectedNode.Tag != null
                            && QualitivityViewPropertiesController.NavigationTreeView.SelectedNode.Tag.GetType() == typeof(CompanyProfile))
                        {

                            #region  |  client  |

                            var cpi = (CompanyProfile)QualitivityViewPropertiesController.NavigationTreeView.SelectedNode.Tag;
                            var pemRate = Tracked.Settings.LanguageRateGroups.FirstOrDefault(pemr => pemr.Id == cpi.ProfileRate.LanguageRateId);
                            var client = new Client {ClientId = cpi.Id.ToString()};



                            var clientAddress = new Address
                            {
                                AddressStreet = cpi.Street,
                                AddressZip = cpi.Zip,
                                AddressCity = cpi.City,
                                AddressState = cpi.State,
                                AddressCountry = cpi.Country
                            };
                            client.ClientAddress = clientAddress;


                            var contactDetails = new ContactDetails
                            {
                                EMail = cpi.Email,
                                WebPage = cpi.Web,
                                Phone = cpi.Phone,
                                FAX = cpi.Fax
                            };
                            client.ContactDetails = contactDetails;

                            client.ClientVat = cpi.VatCode;
                            client.ClientTax = cpi.TaxCode;

                            var defaultRates = new DefaultRates
                            {
                                HrRate = cpi.ProfileRate.HourlyRateRate,
                                HrRateCurrency = cpi.ProfileRate.HourlyRateCurrency,
                                PemRate = pemRate != null ? pemRate.Name : string.Empty
                            };

                            client.DefaultRates = defaultRates;

                            apv.ClientProfile = client;
                            #endregion

                            propertyGrid1.SelectedObject = apv;
                        }
                        else
                        {
                            propertyGrid1.SelectedObject = null;
                        }
                    }
                }
                else
                {
                    propertyGrid1.SelectedObject = null;
                }
            }
            catch
            {
                // ignore
            }

        }



    }
}
