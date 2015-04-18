#region Copyright Notice
/*
 * ConnectSdk.Windows
 * ITvControl.cs
 * 
 * Copyright (c) 2015 LG Electronics.
 * Created by Sorin S. Serban on 20-2-2015,
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
 #endregion
using ConnectSdk.Windows.Core;
using ConnectSdk.Windows.Service.Capability.Listeners;
using ConnectSdk.Windows.Service.Command;

namespace ConnectSdk.Windows.Service.Capability
{
    public interface ITvControl
    {
        ITvControl GetTvControl();
        CapabilityPriorityLevel GetTvControlCapabilityLevel();

        void ChannelUp(ResponseListener listener);
        void ChannelDown(ResponseListener listener);

        void SetChannel(ChannelInfo channelNumber, ResponseListener listener);

        void GetCurrentChannel(ResponseListener listener);
        IServiceSubscription SubscribeCurrentChannel(ResponseListener listener);

        void GetChannelList(ResponseListener listener);

        void GetProgramInfo(ResponseListener listener);
        IServiceSubscription SubscribeProgramInfo(ResponseListener listener);

        void GetProgramList(ResponseListener listener);
        IServiceSubscription SubscribeProgramList(ResponseListener listener);

        void Get3DEnabled(ResponseListener listener);
        void Set3DEnabled(bool enabled, ResponseListener listener);
        IServiceSubscription Subscribe3DEnabled(ResponseListener listener);
    }
}