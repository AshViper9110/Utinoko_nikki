<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Http\JsonResponse;
use Illuminate\Support\Facades\DB;

class UserItemsController extends Controller
{
    // 所持アイテム取得
    public function index(Request $request): JsonResponse
    {
        $id = $request->query('id');
        $elderUserId = $request->query('elder_user_id');

        $query = DB::table('user_items')
            ->select(
                'id',
                'elder_user_id',
                'items_id',
                'quantity',
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
            'items_id' => 'required|integer|exists:items,id',
            'quantity' => 'required|integer|min:1',
        ]);

        $id = DB::table('user_items')->insertGetId([
            'elder_user_id' => $validated['elder_user_id'],
            'items_id' => $validated['items_id'],
            'quantity' => $validated['quantity'],
            'created_at' => now(),
            'updated_at' => now(),
        ]);

        $item = DB::table('user_items')
            ->where('id', $id)
            ->first();

        return response()->json($item, 201);
    }
}
