﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace ToDoSkillTest.Flow
{
    using System;
    using System.Collections.Specialized;
    using System.Threading.Tasks;
    using AdaptiveCards;
    using Microsoft.Bot.Schema;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ToDoSkill.Dialogs.Shared.Resources;
    using ToDoSkill.Dialogs.ShowToDo.Resources;
    using ToDoSkillTest.Flow.Fakes;

    [TestClass]
    public class ShowToDoFlowTests : ToDoBotTestBase
    {
        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
        }

        [TestMethod]
        public async Task EndToEnd()
        {
            await this.GetTestFlow()
                .Send("Show my to dos")
                .AssertReplyOneOf(this.SettingUpOneNote())
                .AssertReply(this.ShowToDoList())
                .AssertReplyOneOf(this.ShowMoreTasks())
                .Send("show next page")
                .AssertReply(this.ShowNextPage())
                .Send("show previous")
                .AssertReply(this.ShowPreviousPage())
                .StartTestAsync();
        }

        private Action<IActivity> ShowToDoList()
        {
            return activity =>
            {
                var messageActivity = activity.AsMessageActivity();
                Assert.AreEqual(messageActivity.Attachments.Count, 1);
                var responseCard = messageActivity.Attachments[0].Content as AdaptiveCard;
                var adaptiveCardTitle = responseCard.Body[0] as AdaptiveTextBlock;
                var toDoChoices = responseCard.Body[1] as AdaptiveChoiceSetInput;
                var toDoChoiceCount = toDoChoices.Choices.Count;
                CollectionAssert.Contains(
                    this.ParseReplies(ToDoSharedResponses.ShowToDoTasks.Replies, new StringDictionary() { { "taskCount", FakeData.FakeToDoItems.Count.ToString() } }),
                    adaptiveCardTitle.Text);
                Assert.AreEqual(toDoChoiceCount, 5);
            };
        }

        private Action<IActivity> ShowNextPage()
        {
            return activity =>
            {
                var messageActivity = activity.AsMessageActivity();
                Assert.AreEqual(messageActivity.Attachments.Count, 1);
                var responseCard = messageActivity.Attachments[0].Content as AdaptiveCard;
                var adaptiveCardTitle = responseCard.Body[0] as AdaptiveTextBlock;
                var toDoChoices = responseCard.Body[1] as AdaptiveChoiceSetInput;
                var toDoChoiceCount = toDoChoices.Choices.Count;
                CollectionAssert.Contains(
                    this.ParseReplies(ShowToDoResponses.ShowNextToDoTasks.Replies, new StringDictionary()),
                    adaptiveCardTitle.Text);
                Assert.AreEqual(toDoChoiceCount, FakeData.FakeToDoItems.Count - 5);
            };
        }

        private Action<IActivity> ShowPreviousPage()
        {
            return activity =>
            {
                var messageActivity = activity.AsMessageActivity();
                Assert.AreEqual(messageActivity.Attachments.Count, 1);
                var responseCard = messageActivity.Attachments[0].Content as AdaptiveCard;
                var adaptiveCardTitle = responseCard.Body[0] as AdaptiveTextBlock;
                var toDoChoices = responseCard.Body[1] as AdaptiveChoiceSetInput;
                var toDoChoiceCount = toDoChoices.Choices.Count;
                CollectionAssert.Contains(
                    this.ParseReplies(ShowToDoResponses.ShowPreviousToDoTasks.Replies, new StringDictionary()),
                    adaptiveCardTitle.Text);
                Assert.AreEqual(toDoChoiceCount, 5);
            };
        }

        private string[] SettingUpOneNote()
        {
            return this.ParseReplies(ToDoSharedResponses.SettingUpOneNoteMessage.Replies, new StringDictionary());
        }

        private string[] ShowMoreTasks()
        {
            return this.ParseReplies(ShowToDoResponses.ShowingMoreTasks.Replies, new StringDictionary());
        }
    }
}