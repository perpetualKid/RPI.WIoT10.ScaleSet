using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using RPI.WIoT10.FEZUtility;
using Devices.Hardware.Actors;
using System.Threading.Tasks;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace RPI.WIoT10.ScaleSet
{
    public sealed class StartupTask : IBackgroundTask
    {
        private BackgroundTaskDeferral deferral;
        private FEZUtilityShield shield;
        private Servo servo;


        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();
            taskInstance.Canceled += TaskInstance_Canceled;

            shield = await FEZUtilityShield.CreateAsync().ConfigureAwait(false);

            servo = new Servo(new PCA9685PWMChannel(shield.PCA9685PWM, (int)FEZUtilityShield.PwmPin.P0));
            servo.SetLimits(650, 2750, 0, 180, 90);
            servo.Disengage();

            int i = 130;
            servo.Position = i;
            await Task.Delay(500);

            while (i <= 180)
            {
                servo.Position = i;
                i += 2;
                await Task.Delay(100);
            }

            servo.Disengage();
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            //a few reasons that you may be interested in.
            switch (reason)
            {
                case BackgroundTaskCancellationReason.Abort:
                    //app unregistered background task (amoung other reasons).
                    break;
                case BackgroundTaskCancellationReason.Terminating:
                    //system shutdown
                    break;
                case BackgroundTaskCancellationReason.ConditionLoss:
                    break;
                case BackgroundTaskCancellationReason.SystemPolicy:
                    break;
            }
            deferral.Complete();
        }

    }
}
