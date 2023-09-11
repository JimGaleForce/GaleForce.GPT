using System;
using System.Linq;
using System.Threading.Tasks;
using GaleForce.GPT.Attributes;

namespace GaleForce.GPT.Services.OpenAI
{
    public class QOpenAIFunction
    {
        public QOpenAIFunctionStage _Stage { get; set; }

        public event EventHandler OnStageChanged;

        public event Func<object, EventArgs, Task> OnArgumentsPopulatedAsync;

        public async Task ChangeStage(QOpenAIFunctionStage newStage)
        {
            _Stage = newStage;
            switch (newStage)
            {
                case QOpenAIFunctionStage.ArgumentsPopulated:
                    if (OnArgumentsPopulatedAsync != null)
                    {
                        await OnArgumentsPopulatedAsync.Invoke(this, EventArgs.Empty);
                    }
                    break;
            }

            if (OnStageChanged != null)
            {
                OnStageChanged(this, EventArgs.Empty);
            }
        }

        public string GetName()
        {
            // Get the class's type
            var type = GetType();

            // Get the AIFunctionAttribute from the class
            var attribute = (AIFunctionAttribute)Attribute.GetCustomAttribute(type, typeof(AIFunctionAttribute));

            // Return the name from the attribute, or null if the attribute doesn't exist
            return attribute?.Name;
        }

        public object GetResult()
        {
            // Get the class's type
            var type = GetType();

            // Get all properties of the class
            var properties = type.GetProperties();

            // Iterate through each property
            foreach (var property in properties)
            {
                // If the property has the AIResultAttribute
                if (Attribute.IsDefined(property, typeof(AIResultAttribute)))
                {
                    // Get an instance of the class to access the property
                    var instance = this;

                    // Return the value of the property, or null if the property is not set
                    return property.GetValue(instance);
                }
            }

            // If no property has the AIResultAttribute, return null
            return null;
        }

        public bool _HasFunctionResponse
        {
            get { return _Stage == QOpenAIFunctionStage.ResponseContentPopulated; }
        }

        public bool _IsReadyToResubmit
        {
            get { return _Stage == QOpenAIFunctionStage.ReadyToRespond; }
        }
    }
}
