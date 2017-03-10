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

using System.Activities.Presentation.Metadata;
using System.ComponentModel;

namespace Net.Appclusive.Workflows.Design
{
    // Interaction logic for BuildModelDesigner.xaml
    public partial class BuildModelDesigner
    {
        public BuildModelDesigner()
        {
            InitializeComponent();
        }

        public static void RegisterMetadata(AttributeTableBuilder attributeTableBuilder)
        {
            attributeTableBuilder.AddCustomAttributes(
                typeof(BuildModel),
                new DesignerAttribute(typeof(BuildModelDesigner)),
                new DescriptionAttribute("Build Model"));
        }
    }
}
