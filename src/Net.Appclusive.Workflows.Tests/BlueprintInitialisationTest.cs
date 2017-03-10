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

using System.Activities;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Net.Appclusive.Workflows.Tests
{
    [TestClass]
    public class BlueprintInitialisationTest
    {
        [TestCategory("SkipOnTeamCity")]
        [TestMethod]
        public void InvokeWorkflowThatCallsBlueprintInitialisationActivityWithValidInputSucceeds()
        {
            // Arrange
            var completed = false;
            AutoResetEvent syncEvent = new AutoResetEvent(false);

            WorkflowApplication workflowApplication = new WorkflowApplication(new WorkflowThatCallsBlueprintInitialisationActivity());
            
            workflowApplication.Completed = delegate (WorkflowApplicationCompletedEventArgs e)
            {
                completed = true;
                syncEvent.Set();
            };

            workflowApplication.Aborted = delegate (WorkflowApplicationAbortedEventArgs e)
            {
                syncEvent.Set();
                throw new WorkflowApplicationAbortedException(e.Reason.Message);
            };

            workflowApplication.OnUnhandledException = delegate (WorkflowApplicationUnhandledExceptionEventArgs e)
            {
                syncEvent.Set();
                return UnhandledExceptionAction.Terminate;
            };
            
            // Act
            workflowApplication.Run();

            syncEvent.WaitOne();

            // Assert
            Assert.IsTrue(completed);
        }
    }
}
