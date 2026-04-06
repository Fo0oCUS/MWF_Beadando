import { useEffect, useState } from 'react'
import { getMyQuizzes } from '../api'
import { navigate, toApiMessage, type Notice } from '../router'
import type { AuthSession, QuizSummary } from '../types'

export function DashboardView({
  authSession,
  onNotice,
}: {
  authSession: AuthSession
  onNotice: (notice: Notice) => void
}) {
  const [quizzes, setQuizzes] = useState<QuizSummary[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    let active = true

    void (async () => {
      try {
        const response = await getMyQuizzes(authSession)
        if (!active) return
        setQuizzes(response)
      } catch (loadError) {
        if (!active) return
        setError(toApiMessage(loadError))
        onNotice({ tone: 'error', text: 'Nem sikerült lekérni a kvízeidet.' })
      } finally {
        if (active) setLoading(false)
      }
    })()

    return () => {
      active = false
    }
  }, [authSession, onNotice])

  return (
    <section className="stack-layout">
      <div className="panel hero-panel">
        <div>
          <p className="eyebrow">Saját kvízek</p>
          <h2>Kvíz létrehozás / indítás</h2>
          <p>Innen hozhatsz létre új kvízt, vagy indíthatsz új session-t.</p>
        </div>
        <button className="primary-button" onClick={() => navigate('/quiz/new')}>
          Új kvíz létrehozása
        </button>
      </div>

      {loading ? <div className="panel">Kvízek betöltése...</div> : null}
      {error ? <div className="panel error-panel">{error}</div> : null}

      <div className="card-grid">
        {quizzes.map((quiz) => (
          <article className="panel quiz-card" key={quiz.id}>
            <div className="quiz-card-top">
              <span className="pill">{quiz.questionCount} kérdés</span>
              <span className={quiz.isPublished ? 'pill success' : 'pill muted'}>
                {quiz.isPublished ? 'Publikált' : 'Piszkozat'}
              </span>
            </div>
            <h3>{quiz.title}</h3>
            <button className="secondary-button" onClick={() => navigate(`/quiz/${quiz.id}`)}>
              Megnyitás
            </button>
          </article>
        ))}
      </div>

      {!loading && quizzes.length === 0 ? (
        <div className="panel empty-panel">
          Még nincs saját kvízed. Készíts egyet, és innen már indítható is lesz.
        </div>
      ) : null}
    </section>
  )
}
