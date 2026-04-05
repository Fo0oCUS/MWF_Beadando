import { useEffect, useRef, useState, type FormEvent } from 'react'
import { getParticipantSession, joinSession, submitParticipantAnswer } from '../api'
import { ResultsPanel } from '../components/ResultsPanel'
import { formatStage, type Notice, toApiMessage } from '../router'
import { loadStoredParticipant, saveStoredParticipant } from '../storage'
import type { SessionState } from '../types'

export function ParticipantSessionView({
  joinCode,
  onNotice,
}: {
  joinCode: string
  onNotice: (notice: Notice) => void
}) {
  const [participantState, setParticipantState] = useState(() => loadStoredParticipant(joinCode))
  const [sessionState, setSessionState] = useState<SessionState | null>(null)
  const [loading, setLoading] = useState(true)
  const [joining, setJoining] = useState(false)
  const [nickname, setNickname] = useState(participantState?.nickname ?? '')
  const [error, setError] = useState<string | null>(null)
  const questionStartedAtRef = useRef<number | null>(null)

  useEffect(() => {
    setParticipantState(loadStoredParticipant(joinCode))
  }, [joinCode])

  useEffect(() => {
    let active = true

    void (async () => {
      try {
        const response = await getParticipantSession(joinCode, participantState?.participantId)
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
  }, [joinCode, participantState?.participantId])

  useEffect(() => {
    if (sessionState?.stage === 'question-open' && sessionState.currentQuestion) {
      questionStartedAtRef.current = Date.now()
    }
  }, [sessionState?.currentQuestion?.id, sessionState?.stage])

  const handleJoin = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    setJoining(true)
    setError(null)

    try {
      const response = await joinSession(joinCode, nickname)
      const stored = {
        participantId: response.participantId,
        nickname: response.nickname,
      }
      saveStoredParticipant(response.joinCode, stored)
      setParticipantState(stored)
      onNotice({ tone: 'success', text: 'Csatlakozas rendben.' })
      const refreshed = await getParticipantSession(response.joinCode, response.participantId)
      setSessionState(refreshed)
    } catch (joinError) {
      setError(toApiMessage(joinError))
    } finally {
      setJoining(false)
      setLoading(false)
    }
  }

  const handleAnswer = async (answerOptionId: number) => {
    if (!sessionState?.viewer) return

    try {
      const responseTimeMs = questionStartedAtRef.current
        ? Math.max(0, Date.now() - questionStartedAtRef.current)
        : 0
      await submitParticipantAnswer(sessionState.viewer.participantId, answerOptionId, responseTimeMs)
      const refreshed = await getParticipantSession(joinCode, sessionState.viewer.participantId)
      setSessionState(refreshed)
    } catch (answerError) {
      onNotice({ tone: 'error', text: toApiMessage(answerError) })
    }
  }

  if (!participantState) {
    return (
      <section className="single-column">
        <div className="panel form-panel">
          <p className="eyebrow">Quiz code: {joinCode}</p>
          <h2>Adj meg egy nicknevet a csatlakozashoz</h2>
          <form className="form-grid" onSubmit={handleJoin}>
            <label>
              Nicknev
              <input
                maxLength={50}
                value={nickname}
                onChange={(event) => setNickname(event.target.value)}
                required
              />
            </label>
            {error ? <p className="form-error">{error}</p> : null}
            <button className="primary-button stretch-button" disabled={joining} type="submit">
              {joining ? 'Beleptetes...' : 'Csatlakozom'}
            </button>
          </form>
        </div>
      </section>
    )
  }

  if (loading) return <div className="panel">Jatekallapot betoltese...</div>
  if (error) return <div className="panel error-panel">{error}</div>
  if (!sessionState) return <div className="panel error-panel">A session nem erheto el.</div>

  const selectedAnswerOptionId = sessionState.viewer?.selectedAnswerOptionId ?? null

  return (
    <section className="stack-layout">
      <div className="panel hero-panel">
        <div>
          <p className="eyebrow">Resztvevoi nezet</p>
          <h2>{sessionState.quizTitle}</h2>
          <p>{participantState.nickname}</p>
        </div>
        <div className="button-row">
          <span className="pill">{formatStage(sessionState.stage)}</span>
        </div>
      </div>

      {sessionState.stage === 'lobby' ? (
        <div className="panel waiting-panel">
          A jatek meg nem indult el. Amint a host aktivalja az elso kerdest, itt meg fog
          jelenni.
        </div>
      ) : null}

      {sessionState.currentQuestion ? (
        <div className="panel">
          <div className="panel-header">
            <h3>
              {sessionState.currentQuestionIndex + 1}. kerdes / {sessionState.totalQuestionCount}
            </h3>
            <span className="pill muted">
              {sessionState.currentQuestion.timeLimitSeconds} mp idolimit
            </span>
          </div>
          <p className="question-text">{sessionState.currentQuestion.text}</p>
          <div className="answer-grid">
            {sessionState.currentQuestion.answerOptions.map((answer) => {
              const isSelected = selectedAnswerOptionId === answer.id
              return (
                <button
                  className={isSelected ? 'answer-card answer-card-selected' : 'answer-card'}
                  disabled={!sessionState.canAnswer}
                  key={answer.id}
                  onClick={() => handleAnswer(answer.id)}
                >
                  <span>{answer.text}</span>
                  {isSelected ? <strong>Leadva</strong> : null}
                </button>
              )
            })}
          </div>
          {!sessionState.canAnswer ? (
            <p className="helper-text">
              {sessionState.viewer?.hasAnsweredCurrentQuestion
                ? 'Erre a kerdesre mar leadtad a valaszodat.'
                : 'Most epp nem lehet valaszolni.'}
            </p>
          ) : null}
        </div>
      ) : null}

      {sessionState.currentQuestionResults ? (
        <ResultsPanel results={sessionState.currentQuestionResults} />
      ) : null}

      {sessionState.stage === 'finished' ? (
        <div className="panel success-panel">
          A kviz veget ert. A vegso pontszamod: <strong>{sessionState.viewer?.totalScore ?? 0}</strong>.
        </div>
      ) : null}
    </section>
  )
}
