import { useState, type FormEvent } from 'react'
import { loginUser, registerUser } from '../api'
import type { AuthSession } from '../types'
import { toApiMessage } from '../router'

export function AuthView({ onLogin }: { onLogin: (session: AuthSession) => void }) {
  const [mode, setMode] = useState<'login' | 'register'>('login')
  const [name, setName] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [submitting, setSubmitting] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const title = mode === 'login' ? 'Bejelentkezés' : 'Regisztráció'

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    setSubmitting(true)
    setError(null)

    try {
      if (mode === 'register') {
        await registerUser({ name, email, password })
      }

      const response = await loginUser({ email, password })
      onLogin(response)
    } catch (submitError) {
      setError(toApiMessage(submitError))
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <section className="single-column">
      <div className="panel form-panel">
        <div className="panel-header">
          <div>
            <h2>{title}</h2>
          </div>
          <div className="segmented-control">
            <button
              className={mode === 'login' ? 'segment active' : 'segment'}
              onClick={() => setMode('login')}
              type="button"
            >
              Belépés
            </button>
            <button
              className={mode === 'register' ? 'segment active' : 'segment'}
              onClick={() => setMode('register')}
              type="button"
            >
              Regisztráció
            </button>
          </div>
        </div>

        <form className="form-grid" onSubmit={handleSubmit}>
          {mode === 'register' ? (
            <label>
              Név
              <input value={name} onChange={(event) => setName(event.target.value)} required />
            </label>
          ) : null}
          <label>
            Email
            <input
              type="email"
              value={email}
              onChange={(event) => setEmail(event.target.value)}
              required
            />
          </label>
          <label>
            Jelszó
            <input
              type="password"
              value={password}
              onChange={(event) => setPassword(event.target.value)}
              required
            />
          </label>

          {error ? <p className="form-error">{error}</p> : null}

          <button className="primary-button stretch-button" disabled={submitting} type="submit">
            {submitting ? 'Folyamatban...' : title}
          </button>
        </form>
      </div>
    </section>
  )
}
