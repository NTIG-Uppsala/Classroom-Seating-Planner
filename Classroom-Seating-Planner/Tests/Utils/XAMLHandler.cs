using System;
using System.Diagnostics;
using System.Dynamic;
using FlaUI.Core.AutomationElements;
using FlaUIElement = FlaUI.Core.AutomationElements;

namespace Tests
{
    internal class XAMLHandler
    {
        public record Options(bool matchWholeString = false)
        {
            public bool matchWholeString { get; set; } = matchWholeString;
        }

        // Returns list of ListBox items
        public static List<string> GetClassListFromElement(FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
        {
            string automationId = "ClassListElement";
            FlaUIElement.ListBox classListElement = window.FindFirstDescendant(cf.ByAutomationId(automationId)).AsListBox();
            if (classListElement == null)
            {
                Trace.WriteLine("ClassListElement is null");
            }
            List<FlaUIElement.ListBoxItem> classListAsItems = classListElement.Items.ToList();
            List<string> classList = classListAsItems.Select(item => item.Text).ToList();
            //List<string> classList = classListElement.Items.Select(item => item.Text).ToList();

            return classList;
        }

        public static void ClickRandomizeSeatingButton(FlaUIElement.Window window, FlaUI.Core.Conditions.ConditionFactory cf)
        {
            // Find and press the randomizer button
            FlaUIElement.AutomationElement randomizeButton = window.FindFirstDescendant(cf.ByAutomationId("RandomizeSeatingButton")).AsButton();
            randomizeButton.Click();
        }

        public static List<FlaUIElement.AutomationElement>?
            GetAllElementsByAutomationId(
                FlaUIElement.Window window,
                FlaUI.Core.Conditions.ConditionFactory cf,
                string? matchString = null,
                FlaUI.Core.Definitions.ControlType? controlType = null,
                Options? options = null
            )
        {
            if (matchString == null)
            {
                return null;
            }

            // Use default options for non-defined values
            options ??= new Options();

            // Find all elements
            List<FlaUIElement.AutomationElement> allElementsInWindow = window.FindAllDescendants(cf.ByFrameworkId("WPF")).ToList();

            // Return all elements where AutomationId contains value or is equal to value
            List<FlaUIElement.AutomationElement> allElementsByAutomationId = allElementsInWindow.Where(element =>
            {

                // Match the string differently depending on the given option
                bool isStringMatched = false;
                if (options.matchWholeString.Equals(true))
                {
                    isStringMatched = element.AutomationId.Equals((string)matchString);
                }
                else if (options.matchWholeString.Equals(false))
                {
                    isStringMatched = element.AutomationId.Contains((string)matchString);
                }

                return
                !string.IsNullOrEmpty(element.AutomationId)
                &&
                element.AutomationId.Contains(matchString)
                &&
                (controlType == null || element.ControlType.Equals(controlType));
            }).ToList();

            return allElementsByAutomationId;
        }

        public static List<FlaUIElement.AutomationElement>?
            GetAllElementsByHelpText(
                FlaUIElement.Window window,
                FlaUI.Core.Conditions.ConditionFactory cf,
                string? key = null,
                object? value = null,
                FlaUI.Core.Definitions.ControlType? controlType = null,
                Options? options = null
            )
        {
            // If neither a value nor a key is passed, return null
            if (key == null && value == null)
            {
                return null;
            }

            // Use default options for non-defined values
            options ??= new Options();

            // Find all elements
            List<FlaUIElement.AutomationElement> allElementsInWindow = window.FindAllDescendants(cf.ByFrameworkId("WPF")).ToList();

            // If only a key is passed, return all elements where HelpText contains key
            if (key != null && value == null)
            {
                List<FlaUIElement.AutomationElement> allElementsByHelpText = allElementsInWindow.Where(element =>
                    !string.IsNullOrEmpty(element.HelpText)
                    &&
                    (
                        element.HelpText.Contains(key)
                        &&
                        !Utils.XAMLHandler.ParseStringToObject(element.HelpText)[key].Equals(null)
                    )
                    &&
                    (controlType == null || element.ControlType.Equals(controlType))
                ).ToList();

                return allElementsByHelpText;
            }
            // If only a value is passed, return all elements where HelpText is equal to value or contains the value
            if (key == null && value != null)
            {
                List<FlaUIElement.AutomationElement> allElementsByHelpText = allElementsInWindow.Where(element =>
                {

                    // Match the string differently depending on the given option
                    bool isStringMatched = false;
                    if (options.matchWholeString.Equals(true))
                    {
                        isStringMatched = element.HelpText.Equals((string)value);
                    }
                    else if (options.matchWholeString.Equals(false))
                    {
                        isStringMatched = element.HelpText.Contains((string)value);
                    }

                    return
                    !string.IsNullOrEmpty(element.HelpText)
                    &&
                    isStringMatched
                    &&
                    (controlType == null || element.ControlType.Equals(controlType));
                }).ToList();

                return allElementsByHelpText;
            }
            // If both a key and a value is passed, parse the HelpText and return all elements where the value of key is equal to value
            if (key != null && value != null)
            {
                List<FlaUIElement.AutomationElement> allElementsByHelpText = allElementsInWindow.Where(element =>
                    !string.IsNullOrEmpty(element.HelpText)
                    &&
                    (
                        element.HelpText.Contains(key)
                        &&
                        Utils.XAMLHandler.ParseStringToObject(element.HelpText)[key].Equals(value)
                    )
                    &&
                    (controlType == null || element.ControlType.Equals(controlType))
                ).ToList();

                return allElementsByHelpText;
            }

            // Make sure all code paths return a value
            return null;
        }

        // Parses a object formatted string and converts it to an object e.g. "x:1; y:5" -> { x: 1, y: 5 }
        // Currenlty supports bool and int data types (falls back to string)
        public static IDictionary<string, object>? ParseStringToObject(string inputString)
        {
            if (string.IsNullOrWhiteSpace(inputString))
            {
                return null; // Input string cannot be null or empty
            }

            IDictionary<string, object> returnObject = new ExpandoObject() as IDictionary<string, object>;

            List<string> properties = inputString.Split(";")
                .Select((property) => property.Trim())
                .ToList();

            foreach (string property in properties)
            {
                List<string> keyValuePair = property.Split(":")
                    .Select((keyOrValue) => keyOrValue.Trim())
                    .ToList();

                // If the property is not in the format "key: value" or if there are any empty strings, throw an exception
                if (keyValuePair.Count != 2 || keyValuePair.Any((element) => string.IsNullOrEmpty(element)))
                {
                    return null; // String is not formatted as an object
                }

                string key = keyValuePair[0];
                string value = keyValuePair[1];

                // Determine the type of the value and convert it dynamically
                if (bool.TryParse(value, out bool boolValue))
                {
                    returnObject[key] = (bool)boolValue;
                }
                // Float works better than int in this case.
                else if (float.TryParse(value, out float floatValue))
                {
                    returnObject[key] = (float)floatValue;
                }
                else
                {
                    // Fallback, just store it as a string
                    returnObject[key] = value;
                }
            }

            return returnObject.ToDictionary();
        }
    }

}
