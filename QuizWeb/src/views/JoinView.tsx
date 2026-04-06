import { useState, type FormEvent } from 'react'
import { joinSession } from '../api'
import { navigate, toApiMessage, type Notice } from '../router'
import { saveStoredParticipant } from '../storage'

export function JoinView({ onNotice }: { onNotice: (notice: Notice) => void }) {
  const [joinCode, setJoinCode] = useState('')
  const [nickname, setNickname] = useState('')
  const [joining, setJoining] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const handleJoin = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    setJoining(true)
    setError(null)

    try {
      const response = await joinSession(joinCode, nickname)
      saveStoredParticipant(response.joinCode, {
        participantId: response.participantId,
        nickname: response.nickname,
      })
      onNotice({ tone: 'success', text: 'Sikeres csatlakozás.' })
      navigate(`/play/${response.joinCode}`)
    } catch (joinError) {
      setError(toApiMessage(joinError))
    } finally {
      setJoining(false)
    }
  }

  return (
    <section className="single-column">
      <div className="panel form-panel">
        <h2>Csatlakozás aktív kvízhez</h2>
        <form className="form-grid" onSubmit={handleJoin}>
          <label>
            PIN / Belépési kód
            <input
              maxLength={6}
              value={joinCode}
              onChange={(event) => setJoinCode(event.target.value.toUpperCase())}
              required
            />
          </label>
          <label>
            Felhasználónév
            <input
              maxLength={50}
              value={nickname}
              onChange={(event) => setNickname(event.target.value)}
              required
            />
          </label>
          {error ? <p className="form-error">{error}</p> : null}
          <button className="primary-button stretch-button" disabled={joining} type="submit">
            {joining ? 'Csatlakozás...' : 'Belépek'}
          </button>
        </form>
      </div>
    </section>
  )
}
