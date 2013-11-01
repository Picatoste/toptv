using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using TopTV.Model;
using Microsoft.Phone.Scheduler;
using System.Collections.ObjectModel;

namespace sdkMVVMCS.ViewModelNS
{
    public class ViewModelAlarm
    {
        private ICommand addAlarmCommand = null;

        public ICommand AddAlarmCommand
        {
            get
            {
                return this.addAlarmCommand;
            }
        }

        private ICommand deleteAlarmCommand = null;

        public ICommand DeleteAlarmCommand
        {
            get
            {
                return this.deleteAlarmCommand;
            }
        }


        public ViewModelAlarm()
        {
            this.addAlarmCommand = new RelayCommand<FeedItemsModel>(this.AddAlarmAction);
            this.deleteAlarmCommand = new RelayCommand<FeedItemsModel>(this.DeleteAlarmAction);
        }


        private void DeleteAlarmAction(FeedItemsModel feedAlarm)
        {
            if (feedAlarm != null)
            {
                Boolean bOK = false;
                try
                {
                    Reminder reminderDelete = ScheduledActionService.GetActions<Reminder>().Where(res => res.Name == feedAlarm.Id).FirstOrDefault<Reminder>();
                    if (reminderDelete != null)
                    {
                        ScheduledActionService.Remove(reminderDelete.Name);
                        bOK = true;
                    }

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("ERROR DeleteAlarmAction - " + ex.Message);
                }
                finally
                {
                    if (bOK)
                    {
                        feedAlarm.Alarm = null;
                    }
                }

                //DeleteALARM!!
            }
        }

        private void AddAlarmAction(FeedItemsModel feedAlarm)
        {
            if (feedAlarm != null)
            {
                Boolean bOK = false;
                try
                {
                    string reminderName = feedAlarm.Id.ToString();
                    Reminder reminder = new Reminder(reminderName);
                    reminder.Title = feedAlarm.Title;
                    reminder.Content = feedAlarm.Canal + " - " + feedAlarm.Summary;
                    reminder.BeginTime = DateTime.ParseExact(feedAlarm.PublishDate, "dd-MM-yyyy HH:mm", System.Globalization.CultureInfo.CurrentCulture);
                    reminder.ExpirationTime = reminder.BeginTime.AddSeconds(10.0);
                    reminder.RecurrenceType = RecurrenceInterval.None;
                    ScheduledActionService.Add(reminder);
                    bOK = true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("ERROR AddAlarmAction - " + ex.Message);
                }
                finally
                {
                    if (bOK)
                    {
                        feedAlarm.Alarm = new AlarmModel();
                    }
                }

                //AddALARM!!
            }
        }

        #region Helpers Alarm
        public static void CheckAlarms(ObservableCollection<FeedItemsModel> feeds)
        {
            List<Reminder> remindersPhone = ScheduledActionService.GetActions<Reminder>().ToList();
            foreach (Reminder reminder in remindersPhone)
            {
                foreach (FeedItemsModel feed in feeds)
                {
                    if (feed.Id == reminder.Name)
                    {
                        feed.Alarm = new AlarmModel();
                    }
                }
            }

        }
        #endregion
    }
}
