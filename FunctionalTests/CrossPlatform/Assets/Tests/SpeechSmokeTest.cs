using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Text;
using System;

#if UNITY_METRO
using UnityEngine.Windows.Speech;
#endif

internal class SpeechSmokeTest : TestBaseSetup
{
#if UNITY_METRO
    // Speech Systems
    private KeywordRecognizer m_Keyword = null;
    private DictationRecognizer m_Dictation = null;
    private GrammarRecognizer m_Grammar = null;

    private string[] keywords = new[] { "one", "car" };

    string m_GrammarFilePath = null;
    string m_GrammarFilePathBadTest = null;

    public bool BadPathTest = false;

    [SetUp]
    public void SetUp()
    {
        PhraseRecognitionSystem.Restart();
    }

    [TearDown]
    public void TearDown()
    {
        PhraseRecognitionSystem.Shutdown();
    }

    [Ignore("Not supported at the moment by Jenkins")]
    [UnityTest]
    public IEnumerator KeywordTest()
    {
        WmrDeviceCheck();
        m_Keyword = new KeywordRecognizer(keywords);
        m_Keyword.OnPhraseRecognized += M_Keyword_OnPhraseRecognized;
        m_Keyword.Start();

        yield return null;

        Assert.IsTrue(m_Keyword.IsRunning, "Keyword Recognizer is not running!");

        yield return null;

        m_Keyword.Stop();

        yield return null;

        Assert.IsFalse(m_Keyword.IsRunning, "Keyword Recognizer is still running!");

        m_Keyword.Dispose();

        yield return new WaitForSeconds(3f);
    }

	[Ignore("Not supported at the moment by Jenkins")]
    [UnityTest]
    public IEnumerator DictationTest()
	{
	    WmrDeviceCheck();
        m_Dictation = new DictationRecognizer(ConfidenceLevel.High, DictationTopicConstraint.Dictation);
        m_Dictation.AutoSilenceTimeoutSeconds = UnityEngine.Random.Range(5f, 100f);
        m_Dictation.InitialSilenceTimeoutSeconds = UnityEngine.Random.Range(5, 100f);
        m_Dictation.DictationHypothesis += M_Dictation_DictationHypothesis;
        m_Dictation.DictationResult += M_Dictation_DictationResult;
        m_Dictation.DictationError += M_Dictation_DictationError;
        m_Dictation.DictationComplete += M_Dictation_DictationComplete;

        m_Dictation.Start();

        yield return new WaitForSeconds(3f);

        Assert.AreNotEqual(m_Dictation.Status, SpeechSystemStatus.Failed, "Dictation has failed!");
        Assert.AreEqual(m_Dictation.Status, SpeechSystemStatus.Running, "Dictation is not running!");

        yield return new WaitForSeconds(3f);

        m_Dictation.Stop();

        yield return new WaitForSeconds(3f);

        Assert.AreEqual(m_Dictation.Status, SpeechSystemStatus.Stopped, "Dictation is still running!");

        yield return null;

        m_Dictation.Dispose();

        yield return null;
    }

	[Ignore("Not supported at the moment by Jenkins")]
    [UnityTest]
    public IEnumerator GrammarTest()
	{
	    WmrDeviceCheck();
        if (PhraseRecognitionSystem.isSupported)
        {
            m_GrammarFilePath = Application.streamingAssetsPath + "/combinations.grxml";
            m_GrammarFilePathBadTest = @"C:\Data\combinations.grxml";

            if (BadPathTest == true)
            {
                m_Grammar = new GrammarRecognizer(m_GrammarFilePathBadTest);
                Debug.Log(m_GrammarFilePathBadTest);
            }
            else
            {
                m_Grammar = new GrammarRecognizer(m_GrammarFilePath);
                Debug.Log(m_GrammarFilePath);
            }

            //Debug.Log(m_GrammarFilePath);
            m_Grammar.OnPhraseRecognized += (args) =>
            {
                var builder = new StringBuilder();
                builder.AppendFormat("{0} ({1}){2}", args.text, args.confidence, Environment.NewLine);

                if (args.semanticMeanings != null)
                {
                    foreach (var meaning in args.semanticMeanings)
                    {
                        builder.AppendFormat("\t{0}{1}", meaning.key, Environment.NewLine);
                        foreach (var value in meaning.values)
                            builder.AppendFormat("\t\t{0}{1}", value, Environment.NewLine);
                    }
                }
                else
                {
                    builder.AppendFormat("\t{0}{1}", "No semantic meaning", Environment.NewLine);
                }

                var lines = builder.ToString();
                Debug.Log(lines);
            };

            m_Grammar.Start();

            yield return new WaitForSeconds(3f);

            Assert.IsTrue(m_Grammar.IsRunning, "Grammar is not running");

            yield return new WaitForSeconds(3f);

            m_Grammar.Stop();

            yield return new WaitForSeconds(3f);

            Assert.IsFalse(m_Grammar.IsRunning, "Grammar is still running!");

            yield return new WaitForSeconds(3f);

            m_Grammar.Dispose();

            yield return new WaitForSeconds(3f);
        }
    }

    private void M_Dictation_DictationComplete(DictationCompletionCause cause)
    {
        Debug.Log("Dictation Complete");
    }

    private void M_Dictation_DictationError(string error, int hresult)
    {
        Debug.Log("Dictation Error");
    }

    private void M_Dictation_DictationResult(string text, ConfidenceLevel confidence)
    {
        Debug.Log("Dictation Result");
    }

    private void M_Dictation_DictationHypothesis(string text)
    {
        Debug.Log("Dictation Hypothesis");
    }

    private void M_Keyword_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        Debug.Log("Keyword Recognized");
    }
#endif
}

