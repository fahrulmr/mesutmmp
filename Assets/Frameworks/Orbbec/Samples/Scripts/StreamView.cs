using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StreamView : MonoBehaviour
{
    

    void Awake()
    {
        StreamViewModel viewModel = StreamViewModel.Instance;

       

        AstraManager.Instance.OnInitializeSuccess.AddListener(() =>
        {
            viewModel.depthStream.Value = true;           

            var pid = AstraManager.Instance.DepthStream.usbInfo.Pid;
            if (pid == Constant.BUS_CL_PID)
            {
            }
            else
            {
                viewModel.colorStream.Value = true;
                viewModel.colorizedBodyStream.Value = true;
                viewModel.bodyStream.Value = true;
                viewModel.maskedColorStream.Value = true;
            }

            AstraManager.Instance.IsBodyOn = true;

        });
    }
}
