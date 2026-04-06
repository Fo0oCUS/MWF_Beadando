import { useEffect, useState } from 'react'
import { createQuizSession, getLatestSessionForQuiz, getQuizDetails } from '../api'
import { formatStage, navigate, toApiMessage, type Notice } from '../router'
import type { AuthSession, QuizDetails, SessionState } from '../types'

export function QuizDetailView({
  authSession,
  quizId,
  onNotice,
}: {
  authSession: AuthSession
  quizId: number
  onNotice: (notice: Notice) => void
}) {
  const [quiz, setQuiz] = useState<QuizDetails | null>(null)
  const [latestSession, setLatestSession] = useState<SessionState | null>(null)
  const [loading, setLoading] = useState(true)
  const [sessionBusy, setSessionBusy] = useState(false)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    let active = true

    void (async () => {
      try {
        const [quizResponse, sessionResponse] = await Promise.all([
          getQuizDetails(quizId, authSession),
          getLatestSessionForQuiz(quizId, authSession).catch(() => null),
        ])
        if (!active) return
        setQuiz(quizResponse)
        setLatestSession(sessionResponse)
      } catch (loadError) {
        if (!active) return
        setError(toApiMessage(loadError))
      } finally {
        if (active) setLoading(false)
      }
    })()

    return () => {
      active = false
    }
  }, [authSession, quizId])

  const handleCreateSession = async () => {
    setSessionBusy(true)
    try {
      const session = await createQuizSession(quizId, authSession)
      setLatestSession(session)
      onNotice({ tone: 'success', text: `Új session létrejött: ${session.joinCode}` })
      navigate(`/session/${session.sessionId}`)
    } catch (createError) {
      onNotice({ tone: 'error', text: toApiMessage(createError) })
    } finally {
      setSessionBusy(false)
    }
  }

  if (loading) return <div className="panel">Kvíz betöltése...</div>
  if (error) return <div className="panel error-panel">{error}</div>
  if (!quiz) return <div className="panel error-panel">A kvíz nem található.</div>

  return (
    <section className="stack-layout">
      <div className="panel hero-panel">
        <div>
          <p className="eyebrow">Kvíz részletei</p>
          <h2>{quiz.title}</h2>
          <p>{quiz.questions.length} kérdés, minden kérdéshez egy helyes opcióval.</p>
        </div>
        <div className="button-row">
          <button className="ghost-button" onClick={() => navigate('/dashboard')}>
            Vissza
          </button>
          <button
            className="primary-button"
            disabled={sessionBusy}
            onClick={handleCreateSession}
          >
            {sessionBusy ? 'Létrehozás...' : 'Új session indítása'}
          </button>
        </div>
      </div>

      {latestSession ? (
        <div className="panel">
          <div className="panel-header">
            <h3>Legutóbbi session</h3>
            <button
              className="secondary-button"
              onClick={() => navigate(`/session/${latestSession.sessionId}`)}
            >
              Legutóbbi kvíz megnyitása
            </button>
          </div>
          <div className="inline-stats">
            <span className="pill"> {latestSession.joinCode}</span>
            <span className="pill muted"> {formatStage(latestSession.stage)}</span>
            <span className="pill"> {latestSession.participantCount} résztvevő</span>
          </div>
        </div>
      ) : null}

      <div className="stack-layout">
        {quiz.questions.map((question, questionIndex) => (
          <article className="panel question-summary" key={question.id}>
            <div className="question-summary-top">
              <span className="pill">{questionIndex + 1}. kérdés</span>
              <span className="pill muted">{question.timeLimitSeconds} mp</span>
            </div>
            <h3>{question.text}</h3>
            <ul className="answer-preview-list">
              {question.answerOptions.map((option) => (
                <li key={option.id}>
                  <span>{option.text}</span>
                  {option.isCorrect ? <strong>Helyes válasz</strong> : <span>---</span>}
                </li>
              ))}
            </ul>
          </article>
        ))}
      </div>
    </section>
  )
}
