using UnityEditor;

namespace ChatGPTWrapper {
  	[CustomEditor(typeof(ChatGPTConversation))]
  	public class ChatGPTEditor : Editor
  	{
		SerializedProperty _apiKey;
		SerializedProperty _model;
		SerializedProperty _maxTokens;
		SerializedProperty _temperature;
		SerializedProperty _chatbotName;
		SerializedProperty _initialPrompt;
		SerializedProperty _firstQuestion;
		SerializedProperty _firstAnswer;
		SerializedProperty chatGPTResponse;


		private void OnEnable() 
		{
			_apiKey = serializedObject.FindProperty("_apiKey");
			_model = serializedObject.FindProperty("_model");
			_maxTokens = serializedObject.FindProperty("_maxTokens");
			_temperature = serializedObject.FindProperty("_temperature");
			_chatbotName = serializedObject.FindProperty("_chatbotName");
			_initialPrompt = serializedObject.FindProperty("_initialPrompt");
			_firstQuestion = serializedObject.FindProperty("_initialQuestion");
			_firstAnswer = serializedObject.FindProperty("_initialAnswer");
			chatGPTResponse = serializedObject.FindProperty("chatGPTResponse");

		}
		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			
			EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(_apiKey);
			EditorGUILayout.PropertyField(_model);

			// if (_model.enumValueIndex != 0) {
			EditorGUILayout.PropertyField(_maxTokens);
			EditorGUILayout.PropertyField(_temperature);
			// }

			EditorGUILayout.Space(5);

			EditorGUILayout.LabelField("Prompt", EditorStyles.boldLabel);

			if (_model.enumValueIndex != 0) {
				EditorGUILayout.PropertyField(_chatbotName);
			}
			EditorGUILayout.PropertyField(_initialPrompt);
			EditorGUILayout.PropertyField(_firstQuestion); 
			EditorGUILayout.PropertyField(_firstAnswer); 

			EditorGUILayout.Space(20);

			EditorGUILayout.PropertyField(chatGPTResponse);
			

			serializedObject.ApplyModifiedProperties();
		}
  	}
}