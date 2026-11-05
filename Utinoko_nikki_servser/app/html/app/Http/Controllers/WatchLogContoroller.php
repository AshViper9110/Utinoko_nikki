<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Http\JsonResponse;
use Illuminate\Support\Facades\DB;

class WatchLogContoroller extends Controller
{
    // 所持アイテム取得
    public function index(Request $request): JsonResponse
    {
        $id = $request->query('id');
        $elderUserId = $request->query('elder_user_id');

        $query = DB::table('watch_logs')
            ->select(
                'id',
                'elder_user_id',
                'videos_id',
                'created_at',
                'updated_at'
            );

        if ($id !== null && $id !== '') {
            $item = $query->where('id', $id)->first();

            return response()->json($item);
        }

        if ($elderUserId !== null && $elderUserId !== '') {
            $query->where('elder_user_id', $elderUserId);
        }

        return response()->json($query->get());
    }

    // 所持アイテム登録
    public function store(Request $request): JsonResponse
    {
        $validated = $request->validate([
            'elder_user_id' => 'required|integer|exists:elder_users,id',
            'videos_id' => 'required|integer|exists:videos,id',
        ]);

        $id = DB::table('watch_logs')->insertGetId([
            'elder_user_id' => $validated['elder_user_id'],
            'videos_id' => $validated['videos_id'],
            'created_at' => now(),
            'updated_at' => now(),
        ]);

        $item = DB::table('watch_logs')
            ->where('id', $id)
            ->first();

        return response()->json($item, 201);
    }
}
