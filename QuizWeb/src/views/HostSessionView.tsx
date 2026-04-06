import { useEffect, useState } from 'react'
import { closeQuestion, getHostSession, nextQuestion, startSession } from '../api'
import { ResultsPanel } from '../components/ResultsPanel'
import { formatStage, type Notice, toApiMessage } from '../router'
import type { AuthSession, SessionState } from '../types'

export function HostSessionView({
  authSession,
  sessionId,
  onNotice,
}: {
  authSession: AuthSession
  sessionId: number
  onNotice: (notice: Notice) => void
}) {
  const [sessionState, setSessionState] = useState<SessionState | null>(null)
  const [loading, setLoading] = useState(true)
  const [busyAction, setBusyAction] = useState<string | null>(null)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    let active = true

    void (async () => {
      try {
        const response = await getHostSession(sessionId, authSession)
        if (!active) return
        setSessionState(response)
        setError(null)
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
  }, [authSession, sessionId])

  const runAction = async (
    actionName: string,
    action: (currentSessionId: number, currentAuth: AuthSession) => Promise<SessionState>,
    successMessage: string,
  ) => {
    setBusyAction(actionName)
    try {
      const updatedState = await action(sessionId, authSession)
      setSessionState(updatedState)
      onNotice({ tone: 'success', text: successMessage })
    } catch (actionError) {
      onNotice({ tone: 'error', text: toApiMessage(actionError) })
    } finally {
      setBusyAction(null)
    }
  }

  if (loading) return <div className="panel">Kvíz betöltése...</div>
  if (error) return <div className="panel error-panel">{error}</div>
  if (!sessionState) return <div className="panel error-panel">A kvíz nem található.</div>

  return (
    <section className="stack-layout">
      <div className="panel hero-panel">
        <div>
          <h2>{sessionState.quizTitle}</h2>
          <p>
            PIN kód: <strong>{sessionState.joinCode}</strong>
          </p>
        </div>
        <div className="button-row">
          {sessionState.stage === 'lobby' ? (
            <button
              className="primary-button"
              disabled={busyAction === 'start'}
              onClick={() => runAction('start', startSession, 'A kvíz elindult.')}
            >
              {busyAction === 'start' ? 'Indítás...' : 'Kvíz indítása'}
            </button>
          ) : null}
          {sessionState.stage === 'question-open' ? (
            <button
              className="primary-button"
              disabled={busyAction === 'close'}
              onClick={() => runAction('close', closeQuestion, 'A kérdés lezárva.')}
            >
              {busyAction === 'close' ? 'Lezárás...' : 'Kérdés lezárása'}
            </button>
          ) : null}
          {sessionState.stage === 'question-closed' ? (
            <button
              className="primary-button"
              disabled={busyAction === 'next'}
              onClick={() => runAction('next', nextQuestion, 'A következő kérdés aktív.')}
            >
              {busyAction === 'next' ? 'Léptetés...' : 'Következő kérdés'}
            </button>
          ) : null}
        </div>
      </div>

      <div className="stats-grid">
        <div className="panel">
          <span className="metric-label">Állapot</span>
          <strong> {formatStage(sessionState.stage)}</strong>
        </div>
        <div className="panel">
          <span className="metric-label">Haladás</span>
          <strong> {Math.max(sessionState.currentQuestionIndex + 1, 0)}/{sessionState.totalQuestionCount}</strong>
        </div>
        <div className="panel">
          <span className="metric-label">Résztvevők</span>
          <strong> {sessionState.participantCount}</strong>
        </div>
      </div>

      {sessionState.currentQuestion ? (
        <div className="panel">
          <div className="panel-header">
            <h3>Aktuális kérdés</h3>
            <span className="pill muted">
              {sessionState.currentQuestion.timeLimitSeconds} mp időlimit
            </span>
          </div>
          <p className="question-text">{sessionState.currentQuestion.text}</p>
          <div className="answer-grid">
            {sessionState.currentQuestion.answerOptions.map((answer) => (
              <div className="answer-card static-answer-card" key={answer.id}>
                <span>{answer.text}</span>
              </div>
            ))}
          </div>
        </div>
      ) : null}

      {sessionState.currentQuestionResults ? (
        <ResultsPanel results={sessionState.currentQuestionResults} />
      ) : null}
    </section>
  )
}
