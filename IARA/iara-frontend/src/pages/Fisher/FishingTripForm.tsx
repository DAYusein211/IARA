import { useState } from 'react';
import { Modal } from '../../components/shared/Modal';
import { Button } from '../../components/shared/Button';
import { fishingTripsAPI } from '../../api/services';
import { Ship, Catch } from '../../types';
  import { useEffect } from 'react';
interface FishingTripFormProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
  ships: Ship[];
  initialTrip?: import('../../types').FishingTrip | null;
  isEdit?: boolean;
}

export const FishingTripForm = ({ isOpen, onClose, onSuccess, ships, initialTrip, isEdit }: FishingTripFormProps) => {
  const [catches, setCatches] = useState<Catch[]>(initialTrip?.catches || []);
  const [catchFishType, setCatchFishType] = useState('');
  const [catchQuantityKg, setCatchQuantityKg] = useState<number | ''>('');
  const [loading, setLoading] = useState(false);

  const addCatch = () => {
    if (!catchFishType || !catchQuantityKg) return;
    setCatches([...catches, { fishType: catchFishType, quantityKg: Number(catchQuantityKg) }]);
    setCatchFishType('');
    setCatchQuantityKg('');
  };

  const removeCatch = (idx: number) => {
    setCatches(catches.filter((_, i) => i !== idx));
  };

  const [startTime, setStartTime] = useState<string>(initialTrip ? initialTrip.startTime.slice(0, 16) : new Date().toISOString().slice(0, 16));


  useEffect(() => {
    if (!isEdit && isOpen) {
      setStartTime(new Date().toISOString().slice(0, 16));
    } else if (isEdit && initialTrip) {
      setStartTime(initialTrip.startTime.slice(0, 16));
    }
  }, [isOpen, isEdit, initialTrip]);
  const [shipId, setShipId] = useState<number>(initialTrip?.shipId || ships[0]?.id || 0);
  const [endTime, setEndTime] = useState<string>(initialTrip?.endTime ? initialTrip.endTime.slice(0, 16) : '');
  const [fuelUsed, setFuelUsed] = useState<number | ''>(initialTrip?.fuelUsed ?? '');
  // ...existing code...

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    try {
      // Convert local datetime string to UTC ISO string
      const toUtcIso = (dt: string) => dt ? new Date(dt).toISOString() : null;
      if (isEdit && initialTrip) {
        await fishingTripsAPI.update(initialTrip.id, {
          shipId,
          // Do NOT send startTime on edit to prevent backend update
          endTime: toUtcIso(endTime) || null,
          fuelUsed: fuelUsed === '' ? null : Number(fuelUsed),
          catches,
        });
      } else {
        await fishingTripsAPI.create({
          shipId,
          startTime: toUtcIso(startTime),
          endTime: toUtcIso(endTime) || null,
          fuelUsed: fuelUsed === '' ? null : Number(fuelUsed),
          catches,
        });
      }
      onSuccess();
      onClose();
    } catch (error: any) {
      alert(error.response?.data?.message || (isEdit ? 'Failed to update fishing trip' : 'Failed to create fishing trip'));
    } finally {
      setLoading(false);
    }
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose} title={isEdit ? "Edit Fishing Trip" : "Log New Fishing Trip"}>
      <form onSubmit={handleSubmit} className="space-y-4">
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">Ship</label>
          <select
            value={shipId}
            onChange={e => setShipId(Number(e.target.value))}
            className="w-full px-4 py-2 border border-gray-300 rounded-lg"
            required
          >
            {ships.map(ship => (
              <option key={ship.id} value={ship.id}>{ship.name} ({ship.registrationNumber})</option>
            ))}
          </select>
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">Start Time</label>
          <input
            type="datetime-local"
            value={startTime}
            onChange={e => setStartTime(e.target.value)}
            className="w-full px-4 py-2 border border-gray-300 rounded-lg"
            required
            disabled={isEdit}
          />
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">End Time (optional)</label>
          <input
            type="datetime-local"
            value={endTime}
            onChange={e => setEndTime(e.target.value)}
            className="w-full px-4 py-2 border border-gray-300 rounded-lg"
            min={isEdit ? new Date().toISOString().slice(0, 16) : undefined}
          />
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">Fuel Used (liters, optional)</label>
          <input
            type="number"
            value={fuelUsed}
            onChange={e => setFuelUsed(e.target.value === '' ? '' : Number(e.target.value))}
            className="w-full px-4 py-2 border border-gray-300 rounded-lg"
            min={0}
            max={10000}
          />
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">Catches</label>
          <div className="flex space-x-2 mb-2">
            <input
              type="text"
              placeholder="Fish Type"
              value={catchFishType}
              onChange={e => setCatchFishType(e.target.value)}
              className="flex-1 px-4 py-2 border border-gray-300 rounded-lg"
            />
            <input
              type="number"
              placeholder="Quantity (kg)"
              value={catchQuantityKg}
              onChange={e => setCatchQuantityKg(e.target.value === '' ? '' : Number(e.target.value))}
              className="w-32 px-4 py-2 border border-gray-300 rounded-lg"
              min={0}
              max={10000}
            />
            <Button type="button" onClick={addCatch} disabled={!catchFishType || !catchQuantityKg}>Add</Button>
          </div>
          {catches.length > 0 && (
            <ul className="list-disc pl-5 space-y-1">
              {catches.map((c, idx) => (
                <li key={idx} className="flex items-center justify-between">
                  <span>{c.fishType} - {c.quantityKg} kg</span>
                  <Button type="button" variant="danger" size="sm" onClick={() => removeCatch(idx)}>Remove</Button>
                </li>
              ))}
            </ul>
          )}
        </div>
        <div className="flex justify-end space-x-2 pt-4">
          <Button type="submit" disabled={loading}>{isEdit ? "Save Changes" : "Log Trip"}</Button>
          <Button type="button" variant="secondary" onClick={onClose}>Cancel</Button>
        </div>
      </form>
    </Modal>);
}