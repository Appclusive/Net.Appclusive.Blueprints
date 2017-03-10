/**
 * Copyright 2017 d-fens GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Configuration;
using Net.Appclusive.Public.Constants;

namespace Net.Appclusive.Workflows
{
    public class MessagingClientConfigurationSection
        : ConfigurationSection
    {
        //<configuration>

        //<!-- Configuration section-handler declaration area. -->
        //  <configSections>
        //  	<section 
        //  		name="messageClientSettingsConfiguration" 
        //  		type="Net.Appclusive.Workflows.MessagingClientConfigurationSection, Net.Appclusive.Workflows" 
        //  		allowLocation="true" 
        //  		allowDefinition="Everywhere" 
        //  	/>
        //      <!-- Other <section> and <sectionGroup> elements. -->
        //  </configSections>

        //  <!-- Configuration section settings area. -->
        //  <messageClientSettingsConfiguration clientType="Net.Appclusive.Public.Messaging.MessageQueue" />

        //</configuration>

        [ConfigurationProperty(nameof(Messaging.clientType), DefaultValue = Messaging.clientType, IsRequired = false)]
        public string ClientType
        {
            get { return (string) this[nameof(Messaging.clientType)]; }
            set { this[nameof(Messaging.clientType)] = value; }
        }
    }
}
