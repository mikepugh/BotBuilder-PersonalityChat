// 
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
// 
// Microsoft Bot Framework: http://botframework.com
// 
// Personality Chat based Dialogs for Bot Builder:
// https://github.com/Microsoft/BotBuilder-PersonalityChat
// 
// Copyright (c) Microsoft Corporation
// All rights reserved.
// 
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using Microsoft.Bot.Builder.PersonalityChat.Core;

namespace Microsoft.Bot.Builder.PersonalityChat.Tests
{
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Adapters;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PersonalityChatMiddlewareTests
    {
        [TestMethod]
        [TestCategory("PersonalityChatMiddleware")]
        public async Task PersonalityChat_TestMiddleware()
        {
            TestAdapter adapter = new TestAdapter()
                .Use(new PersonalityChatMiddleware(new PersonalityChatMiddlewareOptions(
                    respondOnlyIfChat: false,
                    endActivityRoutingOnResponse: false)));

            await new TestFlow(adapter, async (context, cancellationToken) =>
            {
                if (context.Activity.Text == "test query aswedff")
                {
                    await context.SendActivityAsync(context.Activity.Text);
                }
            })
                .Send("test query aswedff")
                    .AssertReply("test response")
                .StartTestAsync();
        }

        [TestMethod]
        [TestCategory("PersonalityChatMiddleware")]
        public async Task PersonalityChat_TestEndRouting_True()
        {
            TestAdapter adapter = new TestAdapter()
                .Use(new PersonalityChatMiddleware(new PersonalityChatMiddlewareOptions(
                    endActivityRoutingOnResponse: true)));

            await new TestFlow(adapter, async (context, cancellationToken) => await context.SendActivityAsync(context.Activity.Text))
                .Send("Hello")
                .AssertReply("Hey. What's up?")
                .StartTestAsync();
        }

        [TestMethod]
        [TestCategory("PersonalityChatMiddleware")]
        public async Task PersonalityChat_TestEndRouting_False()
        {
            TestAdapter adapter = new TestAdapter()
                .Use(new PersonalityChatMiddleware(
                    new PersonalityChatMiddlewareOptions(
                        endActivityRoutingOnResponse: false)));

            await new TestFlow(adapter, async (context, cancellationToken) =>
            {
                await context.SendActivityAsync(context.Activity.Text);
            })
                .Send("Hello")
                .AssertReply("Hey. What's up?")
                .AssertReply("Hello")
                .StartTestAsync();
        }

        [TestMethod]
        [TestCategory("PersonalityChatMiddleware")]
        public async Task PersonalityChat_TestEndRouting_Switch_Persona()
        {
            TestAdapter adapter = new TestAdapter()
                .Use(new PersonalityChatMiddleware(
                    new PersonalityChatMiddlewareOptions(
                        botPersona: PersonalityChatPersona.Professional)));

            await new TestFlow(adapter, async (context, cancellationToken) =>
            {
                await context.SendActivityAsync(context.Activity.Text);
            })
                .Send("Hello")
                .AssertReply("Hello. What can I do for you?")
                .StartTestAsync();

            adapter = new TestAdapter()
                .Use(new PersonalityChatMiddleware(
                    new PersonalityChatMiddlewareOptions(
                        botPersona: PersonalityChatPersona.Friendly)));

            await new TestFlow(adapter, async (context, cancellationToken) =>
            {
                await context.SendActivityAsync(context.Activity.Text);
            })
                .Send("Hello")
                .AssertReply("Hey. What's up?")
                .StartTestAsync();
        }

    }
}
