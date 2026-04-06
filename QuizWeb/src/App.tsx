import { startTransition, useEffect, useState, type ReactNode } from 'react'
import './App.css'
import { clearAuthSession, loadAuthSession, saveAuthSession } from './storage'
import type { AuthSession } from './types'
import { AuthRequiredCard } from './components/AuthRequiredCard'
import { AuthView } from './views/AuthView'
import { DashboardView } from './views/DashboardView'
import { HomeView } from './views/HomeView'
import { HostSessionView } from './views/HostSessionView'
import { JoinView } from './views/JoinView'
import { ParticipantSessionView } from './views/ParticipantSessionView'
import { QuizBuilderView } from './views/QuizBuilderView'
import { QuizDetailView } from './views/QuizDetailView'
import { navigate, parseRoute, type Notice, type Route } from './router'

function App() {
  const [authSession, setAuthSession] = useState<AuthSession | null>(() => loadAuthSession())
  const [route, setRoute] = useState<Route>(() => parseRoute(window.location.hash))
  const [notice, setNotice] = useState<Notice | null>(null)

  useEffect(() => {
    const onHashChange = () => {
      startTransition(() => setRoute(parseRoute(window.location.hash)))
    }

    window.addEventListener('hashchange', onHashChange)
    return () => window.removeEventListener('hashchange', onHashChange)
  }, [])

  useEffect(() => {
    if (!notice) return undefined
    const timeoutId = window.setTimeout(() => setNotice(null), 5000)
    return () => window.clearTimeout(timeoutId)
  }, [notice])

  const handleLogin = (session: AuthSession) => {
    saveAuthSession(session)
    setAuthSession(session)
    setNotice({ tone: 'success', text: 'Sikeres bejelentkezés.' })
    navigate('/dashboard')
  }

  const handleLogout = () => {
    clearAuthSession()
    setAuthSession(null)
    setNotice({ tone: 'success', text: 'Kijelentkeztél.' })
    navigate('/')
  }

  const requireAuth = (view: ReactNode) => (authSession ? view : <AuthRequiredCard />)

  return (
    <div className="app-shell">
      <header className="topbar">
        <button className="brand" onClick={() => navigate('/')}>
          MWF Kvíz
        </button>
        <nav className="topbar-actions">
          <button className="ghost-button" onClick={() => navigate('/join')}>
            Csatlakozás
          </button>
          <button
            className="ghost-button"
            onClick={() => navigate(authSession ? '/dashboard' : '/auth')}
          >
            Kvízeim
          </button>
          {authSession ? (
            <button className="primary-button" onClick={handleLogout}>
              Kijelentkezés
            </button>
          ) : (
            <button className="primary-button" onClick={() => navigate('/auth')}>
              Bejelentkezés
            </button>
          )}
        </nav>
      </header>

      {notice ? (
        <div className={`notice notice-${notice.tone}`}>
          <span>{notice.text}</span>
          <button onClick={() => setNotice(null)}>Bezárás</button>
        </div>
      ) : null}

      <main className="page-shell">
        {route.name === 'home' ? (
          <HomeView onOpenDashboard={() => navigate(authSession ? '/dashboard' : '/auth')} />
        ) : null}
        {route.name === 'auth' ? <AuthView onLogin={handleLogin} /> : null}
        {route.name === 'dashboard'
          ? requireAuth(<DashboardView authSession={authSession!} onNotice={setNotice} />)
          : null}
        {route.name === 'quiz-new'
          ? requireAuth(<QuizBuilderView authSession={authSession!} onNotice={setNotice} />)
          : null}
        {route.name === 'quiz-detail'
          ? requireAuth(
              <QuizDetailView authSession={authSession!} quizId={route.quizId} onNotice={setNotice} />,
            )
          : null}
        {route.name === 'host-session'
          ? requireAuth(
              <HostSessionView
                authSession={authSession!}
                sessionId={route.sessionId}
                onNotice={setNotice}
              />,
            )
          : null}
        {route.name === 'join' ? <JoinView onNotice={setNotice} /> : null}
        {route.name === 'play' ? (
          <ParticipantSessionView joinCode={route.joinCode} onNotice={setNotice} />
        ) : null}
      </main>
    </div>
  )
}

export default App
