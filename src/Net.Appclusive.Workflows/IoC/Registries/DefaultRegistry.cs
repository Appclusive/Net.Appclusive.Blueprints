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

using System;
using System.Configuration;
using System.Linq;
using Net.Appclusive.Public.Constants;
using Net.Appclusive.Public.Messaging;
using StructureMap;

namespace Net.Appclusive.Workflows.IoC.Registries
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            var messagingClientConfigurationSection = 
                (MessagingClientConfigurationSection) ConfigurationManager.GetSection(Messaging.SECTION_NAME) 
                ?? new MessagingClientConfigurationSection();

            var clientTypeName = messagingClientConfigurationSection.ClientType;
            var clientType = default(Type);

            Scan(scanner =>
            {
                scanner.AssembliesFromPath(IoC.AssemblyDirectory, assembly =>
                {
                    if (null != clientType)
                    {
                        return false;
                    }

                    clientType = assembly.DefinedTypes.FirstOrDefault(type => type.IsPublic
                        && typeof(IMessagingClient).IsAssignableFrom(type)
                        && type.FullName == clientTypeName);

                    return null != clientType;
                });
                scanner.WithDefaultConventions();
            });

            For<IMessagingClient>().Use(ctx => (IMessagingClient) ctx.GetInstance(clientType))
                .Singleton();
        }
    }
}
