import { useState, type FormEvent } from 'react'
import { createQuiz } from '../api'
import { navigate, toApiMessage, type Notice } from '../router'
import type { AuthSession, CreateQuizRequest } from '../types'

type QuizDraft = {
  title: string
  isPublished: boolean
  questions: Array<{
    text: string
    timeLimitSeconds: number
    options: Array<{
      text: string
      isCorrect: boolean
    }>
  }>
}

function defaultQuizDraft(): QuizDraft {
  return {
    title: '',
    isPublished: true,
    questions: [
      {
        text: '',
        timeLimitSeconds: 20,
        options: [
          { text: '', isCorrect: true },
          { text: '', isCorrect: false },
        ],
      },
    ],
  }
}

export function QuizBuilderView({
  authSession,
  onNotice,
}: {
  authSession: AuthSession
  onNotice: (notice: Notice) => void
}) {
  const [draft, setDraft] = useState<QuizDraft>(() => defaultQuizDraft())
  const [submitting, setSubmitting] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const updateQuestion = (
    questionIndex: number,
    updater: (question: QuizDraft['questions'][number]) => QuizDraft['questions'][number],
  ) => {
    setDraft((current) => ({
      ...current,
      questions: current.questions.map((question, index) =>
        index === questionIndex ? updater(question) : question,
      ),
    }))
  }

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    setSubmitting(true)
    setError(null)

    const payload: CreateQuizRequest = {
      title: draft.title.trim(),
      isPublished: draft.isPublished,
      questions: draft.questions.map((question, questionIndex) => ({
        text: question.text.trim(),
        orderIndex: questionIndex,
        timeLimitSeconds: question.timeLimitSeconds,
        answerOptions: question.options.map((option, optionIndex) => ({
          text: option.text.trim(),
          orderIndex: optionIndex,
          isCorrect: option.isCorrect,
        })),
      })),
    }

    try {
      const quiz = await createQuiz(payload, authSession)
      onNotice({ tone: 'success', text: 'A kvíz elkészült.' })
      navigate(`/quiz/${quiz.id}`)
    } catch (submitError) {
      setError(toApiMessage(submitError))
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <section className="single-column">
      <form className="stack-layout" onSubmit={handleSubmit}>
        <div className="panel form-panel">
          <div className="panel-header">
            <div>
              <p className="eyebrow">Kvíztervező</p>
              <h2>Új quiz összeállítása</h2>
            </div>
            <button className="ghost-button" onClick={() => navigate('/dashboard')} type="button">
              Vissza
            </button>
          </div>

          <label>
            Cím
            <input
              value={draft.title}
              onChange={(event) =>
                setDraft((current) => ({ ...current, title: event.target.value }))
              }
              minLength={3}
              maxLength={50}
              required
            />
          </label>

          <label className="checkbox-row">
            <input
              checked={draft.isPublished}
              onChange={(event) =>
                setDraft((current) => ({ ...current, isPublished: event.target.checked }))
              }
              type="checkbox"
            />
            Publikáltként mentés
          </label>
        </div>

        {draft.questions.map((question, questionIndex) => (
          <div className="panel question-editor" key={`question-${questionIndex}`}>
            <div className="panel-header">
              <h3>{questionIndex + 1}. kérdés</h3>
              {draft.questions.length > 1 ? (
                <button
                  className="ghost-button danger"
                  onClick={() =>
                    setDraft((current) => ({
                      ...current,
                      questions: current.questions.filter((_, index) => index !== questionIndex),
                    }))
                  }
                  type="button"
                >
                  Törlés
                </button>
              ) : null}
            </div>

            <label>
              Kérdés szövege
              <input
                value={question.text}
                onChange={(event) =>
                  updateQuestion(questionIndex, (currentQuestion) => ({
                    ...currentQuestion,
                    text: event.target.value,
                  }))
                }
                minLength={3}
                maxLength={255}
                required
              />
            </label>

            <label>
              Időlimit másodpercben
              <input
                min={1}
                type="number"
                value={question.timeLimitSeconds}
                onChange={(event) =>
                  updateQuestion(questionIndex, (currentQuestion) => ({
                    ...currentQuestion,
                    timeLimitSeconds: Number.parseInt(event.target.value, 10) || 1,
                  }))
                }
                required
              />
            </label>

            <div className="option-list">
              {question.options.map((option, optionIndex) => (
                <div className="option-editor" key={`option-${optionIndex}`}>
                  <label>
                    Válaszlehetőség
                    <input
                      value={option.text}
                      onChange={(event) =>
                        updateQuestion(questionIndex, (currentQuestion) => ({
                          ...currentQuestion,
                          options: currentQuestion.options.map((currentOption, index) =>
                            index === optionIndex
                              ? { ...currentOption, text: event.target.value }
                              : currentOption,
                          ),
                        }))
                      }
                      required
                    />
                  </label>
                  <label className="checkbox-row">
                    <input
                      checked={option.isCorrect}
                      onChange={() =>
                        updateQuestion(questionIndex, (currentQuestion) => ({
                          ...currentQuestion,
                          options: currentQuestion.options.map((currentOption, index) => ({
                            ...currentOption,
                            isCorrect: index === optionIndex,
                          })),
                        }))
                      }
                      type="radio"
                      name={`correct-${questionIndex}`}
                    />
                    Helyes válasz
                  </label>
                </div>
              ))}
            </div>

            <button
              className="ghost-button"
              onClick={() =>
                updateQuestion(questionIndex, (currentQuestion) => ({
                  ...currentQuestion,
                  options: [...currentQuestion.options, { text: '', isCorrect: false }],
                }))
              }
              type="button"
            >
              Új válasz hozzáadása
            </button>
          </div>
        ))}

        {error ? <div className="panel error-panel">{error}</div> : null}

        <div className="panel action-row">
          <button
            className="secondary-button"
            onClick={() =>
              setDraft((current) => ({
                ...current,
                questions: [
                  ...current.questions,
                  {
                    text: '',
                    timeLimitSeconds: 20,
                    options: [
                      { text: '', isCorrect: true },
                      { text: '', isCorrect: false },
                    ],
                  },
                ],
              }))
            }
            type="button"
          >
            Új kérdés
          </button>
          <button className="primary-button" disabled={submitting} type="submit">
            {submitting ? 'Mentés...' : 'Kvíz mentése'}
          </button>
        </div>
      </form>
    </section>
  )
}
