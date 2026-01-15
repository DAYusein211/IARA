import { useState, useEffect } from 'react';
import { useAuth } from '../../../context/AuthContext';
import { Modal } from '../../../components/shared/Modal';
import { Button } from '../../../components/shared/Button';
import { inspectionsAPI } from '../../../api/services';


import { TargetType, InspectionResult, Inspection } from '../../../types';

type InspectionFormProps = {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
  editInspection?: Inspection | null;
};

export const InspectionForm = ({ isOpen, onClose, onSuccess, editInspection }: InspectionFormProps) => {
  const { user } = useAuth();
  const [targetId, setTargetId] = useState<number>(0);
  const [inspectionDate, setInspectionDate] = useState<string>(new Date().toISOString().slice(0, 10));
  const [result, setResult] = useState<InspectionResult>(InspectionResult.PASSED);
  const [notes, setNotes] = useState('');
  const [fineAmount, setFineAmount] = useState<number | ''>('');
  const [fineReason, setFineReason] = useState('');
  const [loading, setLoading] = useState(false);
  const [targetType, setTargetType] = useState<TargetType>(TargetType.SHIP);

  // Pre-fill fields when editing
  useEffect(() => {
    if (isOpen && editInspection) {
      setTargetId(editInspection.targetId);
      setInspectionDate(editInspection.inspectionDate.slice(0, 10));
      setResult(editInspection.result);
      setNotes(editInspection.notes || '');
      setTargetType(editInspection.targetType);
      setFineAmount(editInspection.fine ? editInspection.fine.amount : '');
      setFineReason(editInspection.fine ? editInspection.fine.reason : '');
    } else if (isOpen && !editInspection) {
      setTargetId(0);
      setInspectionDate(new Date().toISOString().slice(0, 10));
      setResult(InspectionResult.PASSED);
      setNotes('');
      setTargetType(TargetType.SHIP);
      setFineAmount('');
      setFineReason('');
    }
  }, [isOpen, editInspection]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    try {
      // Use full ISO string for inspectionDate
      const isoDate = new Date(inspectionDate).toISOString();
      // Only send fine if both fields are valid
      const fine = fineAmount !== '' && fineReason.trim() !== ''
        ? { amount: Number(fineAmount), reason: fineReason }
        : null;
      if (editInspection && editInspection.id) {
        await inspectionsAPI.update(editInspection.id, {
          inspectorId: user?.id || 0,
          targetType,
          targetId,
          inspectionDate: isoDate,
          result,
          notes,
          fine,
        });
      } else {
        await inspectionsAPI.create({
          inspectorId: user?.id || 0,
          targetType,
          targetId,
          inspectionDate: isoDate,
          result,
          notes,
          fine,
        });
      }
      onSuccess();
      onClose();
    } catch (error: any) {
      let reason = editInspection ? 'Failed to update inspection' : 'Failed to create inspection';
      if (error.response) {
        if (error.response.data?.message) {
          reason += `: ${error.response.data.message}`;
        } else if (typeof error.response.data === 'string') {
          reason += `: ${error.response.data}`;
        } else if (error.response.status) {
          reason += ` (Status ${error.response.status})`;
        }
      } else if (error.message) {
        reason += `: ${error.message}`;
      }
      alert(reason);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose} title={editInspection ? 'Edit Inspection' : 'New Inspection Report'}>
      <form onSubmit={handleSubmit} className="space-y-4">
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">Target Type</label>
          <select
            value={targetType}
            onChange={e => setTargetType(e.target.value as TargetType)}
            className="w-full px-4 py-2 border border-gray-300 rounded-lg"
          >
            {Object.values(TargetType).map(type => (
              <option key={type} value={type}>{type}</option>
            ))}
          </select>
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">Target ID</label>
          <input
            type="number"
            value={targetId}
            onChange={e => setTargetId(Number(e.target.value))}
            className="w-full px-4 py-2 border border-gray-300 rounded-lg"
            min={0}
            required
          />
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">Inspection Date</label>
          <input
            type="date"
            value={inspectionDate}
            onChange={e => setInspectionDate(e.target.value)}
            className="w-full px-4 py-2 border border-gray-300 rounded-lg"
            required
          />
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">Result</label>
          <select
            value={result}
            onChange={e => setResult(e.target.value as InspectionResult)}
            className="w-full px-4 py-2 border border-gray-300 rounded-lg"
          >
            {Object.values(InspectionResult).map(res => (
              <option key={res} value={res}>{res}</option>
            ))}
          </select>
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">Notes</label>
          <textarea
            value={notes}
            onChange={e => setNotes(e.target.value)}
            className="w-full px-4 py-2 border border-gray-300 rounded-lg min-h-20 max-h-40"
            rows={3}
          />
        </div>
        <div className="grid grid-cols-2 gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">Fine Amount (optional)</label>
            <input
              type="number"
              value={fineAmount}
              onChange={e => setFineAmount(e.target.value === '' ? '' : Number(e.target.value))}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg"
              min={0}
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">Fine Reason (optional)</label>
            <input
              type="text"
              value={fineReason}
              onChange={e => setFineReason(e.target.value)}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg"
            />
          </div>
        </div>
        <div className="flex space-x-3 pt-4">
          <Button type="submit" className="flex-1" disabled={loading}>
            {loading ? 'Saving...' : (editInspection ? 'Update Inspection' : 'Create Inspection')}
          </Button>
          <Button type="button" variant="secondary" onClick={onClose} className="flex-1">
            Cancel
          </Button>
        </div>
      </form>
    </Modal>
  );
};
